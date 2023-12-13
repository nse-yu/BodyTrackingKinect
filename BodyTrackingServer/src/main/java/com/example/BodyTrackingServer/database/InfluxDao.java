package com.example.BodyTrackingServer.database;

import com.example.BodyTrackingServer.constants.InfluxConstants;
import com.example.BodyTrackingServer.model.BodyTrackingHabit;
import com.example.BodyTrackingServer.model.BodyTrackingScore;
import com.example.BodyTrackingServer.model.InfluxDBConfig;
import com.influxdb.client.InfluxDBClient;
import com.influxdb.client.InfluxDBClientFactory;
import com.influxdb.client.WriteApiBlocking;
import com.influxdb.client.domain.WritePrecision;
import com.influxdb.client.write.Point;
import com.influxdb.query.FluxColumn;
import com.influxdb.query.FluxTable;
import jakarta.annotation.PostConstruct;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.time.ZoneId;
import java.util.List;
import java.util.Objects;
import java.util.function.Consumer;
import java.util.stream.Collectors;

/** 例外処理について、、、
 * このクラス内のみで使用される関数：例外はここで処理する
 * 別のクラスでも使用される関数：状況にもよるが、基本的に例外は上に投げる
 * */
@Component
public class InfluxDao
{
    public static final Logger log = LoggerFactory.getLogger(InfluxDao.class);

    @Autowired
    private InfluxDBConfig config;
    private InfluxDBClient influxClient;
    private static String usedBucket = "";
    private static final WritePrecision WRITE_PRECISION = WritePrecision.NS;


    public static void setUsedBucket(String bucket)
    {
        usedBucket = bucket;
    }
    public InfluxDao setBucket(String bucket)
    {
        setUsedBucket(bucket);
        return this;
    }


    @PostConstruct
    private void init()
    {
        connect(config);
    }
    public void connect()
    {
        if(config == null)
            log.error("Missing configurations... Please call InfluxDao#connect(InfluxDBConfig config) at first connection.");

        if(usedBucket.isEmpty())
            usedBucket = config.bucket;

        try
        {
            influxClient = InfluxDBClientFactory.create(
                    config.url,
                    config.token.toCharArray(),
                    config.org,
                    usedBucket
            );
        }catch(Exception e)
        {
            log.error(("Failed to connect the influxDB probably due to configurations you specified:\r\n " +
                    "url: %s, username: %s, password: %s [%s]").formatted(config.url, config.username, config.password, e.getMessage()), e.fillInStackTrace());
        }

        if(influxClient == null)
            log.error("Failed to instantiate influxDB, check specified configurations.");

        //enableBatch();

        if (isBucketExists(config.bucket)) return;

        log.error("Bucket named '%s' was not found. Please consider creating it.:\r\n %s".formatted(config.bucket, config));
    }
    public void connect(InfluxDBConfig config)
    {
        this.config = config;
        connect();
    }


    private boolean isBucketExists(String bucket)
    {
        Consumer<FluxTable> showTables = fluxTables ->
        {
            String columnsString = fluxTables.getColumns().stream()
                    .map(FluxColumn::getLabel)
                    .reduce("",(o, n) -> o + "  " + n)
                    .replaceFirst(" ".repeat(2), "");
            String valuesString = fluxTables.getRecords().stream()
                    .map(rec -> rec.getRow().stream()
                            .map(String::valueOf)
                            .reduce("", (o, n) -> o + "  " + n)
                            .replaceFirst(" ".repeat(2), ""))
                    .collect(Collectors.joining(System.lineSeparator()));

            System.out.println("-".repeat(columnsString.length()));
            System.out.println(columnsString);
            System.out.println("-".repeat(columnsString.length()));
            System.out.println(valuesString);
        };
        try {
            String queryString = "buckets()";

            var resultList = influxClient.getQueryApi().query(queryString);
            resultList.forEach(showTables);

            return resultList.get(0).getRecords().stream().anyMatch(rec -> Objects.equals(rec.getValueByKey("name"), bucket));
        } catch (Exception e) {
            log.error("Database availability check failed with the following message: " + e.getMessage());
        }
        return false;
    }
    private boolean isFluxServerAlive()
    {
        try
        {
            influxClient.ping();
            return true;
        } catch (Exception e)
        {
            log.trace("Ping to the influxDB failed.", e);
        }
        return false;
    }
    public boolean isQueryUnavailable()
    {
        return influxClient == null || !isFluxServerAlive();
    }


    public List<String> findBucketAll() throws RuntimeException
    {
        ThrowIfQueryUnavailable();

        String queryString = "buckets()";
        var resultList = influxClient.getQueryApi().query(queryString);
        return resultList.get(0).getRecords().stream()
                .map(rec -> String.valueOf(rec.getValueByKey("name")))
                .map(name -> name.charAt(0) == '_' ? String.format("%s [System]",name) : name)
                .toList();
    }
    public List<BodyTrackingScore> getScores(String model, long start, long stop)
    {
        ThrowIfQueryUnavailable();

        String queryString = String.format("from(bucket: \"%s\") " +
                "|> range(start: -%dm, stop: -%dm)" +
                (model.isEmpty() ? "" : "|> filter(fn: (r) => r.model == \"%s\")".formatted(model)) +
                "|> filter(fn: (r) => r._measurement == \"%s\")".formatted(InfluxConstants.MEASUREMENT_SCORE) +
                "|> pivot(rowKey: [\"_time\"], columnKey: [\"_field\"], valueColumn: \"_value\")" +
                "|> group(columns: [\"_time\", \"userid\", \"model\"])"
                , usedBucket, start, stop);

        var resultList = influxClient.getQueryApi().query(queryString);

        var trackingScoreList = resultList.stream()
                .map(point -> point.getRecords().get(0))
                .map(record -> new BodyTrackingScore(
                        String.valueOf(record.getValueByKey(InfluxConstants.TAG_USER_ID)),
                        String.valueOf(record.getValueByKey(InfluxConstants.TAG_MODEL)),
                        objToFloat(record.getValueByKey(InfluxConstants.FIELD_SCORE)),
                        objToBoolean(record.getValueByKey(InfluxConstants.FIELD_MONITORING)),
                        record.getTime().atZone(InfluxConstants.TIME_ZONE)
                ))
                .toList();

        //System.out.printf("-%d ~ -%d [hour]\n", start, stop);
        trackingScoreList.forEach(System.out::println);

        return trackingScoreList;
    }
    public List<BodyTrackingHabit> getHabits(long start, long stop)
    {
        ThrowIfQueryUnavailable();

        String queryString = String.format("from(bucket: \"%s\") " +
                        "|> range(start: -%dm, stop: -%dm)" +
                        "|> filter(fn: (r) => r._measurement == \"%s\")".formatted(InfluxConstants.MEASUREMENT_HABIT) +
                        "|> pivot(rowKey: [\"_time\"], columnKey: [\"_field\"], valueColumn: \"_value\")" +
                        "|> group(columns: [\"_time\", \"userid\"])"
                , usedBucket, start, stop);

        var resultList = influxClient.getQueryApi().query(queryString);

        var trackingHabits = resultList.stream()
                .map(point -> point.getRecords().get(0))
                .map(record -> new BodyTrackingHabit(
                        String.valueOf(record.getValueByKey(InfluxConstants.TAG_USER_ID)),
                        String.valueOf(record.getValueByKey(InfluxConstants.FIELD_HABIT)),
                        record.getTime().atZone(InfluxConstants.TIME_ZONE)
                ))
                .toList();

        //System.out.printf("-%d ~ -%d [hour]\n", start, stop);
        trackingHabits.forEach(System.out::println);

        return trackingHabits;
    }
    public void saveScores(List<BodyTrackingScore> trackingScores) throws RuntimeException
    {
        ThrowIfQueryUnavailable();

        WriteApiBlocking writeApi = influxClient.getWriteApiBlocking();
        String measurement = InfluxConstants.MEASUREMENT_SCORE;
        trackingScores.forEach(data ->
        {
            System.out.println("Saving...: "+data.toString());
            // write by point
            Point point = Point.measurement(measurement)
                    .addTag(InfluxConstants.TAG_USER_ID, data.getUserId())
                    .addTag(InfluxConstants.TAG_MODEL, data.getModel())
                    .addField(InfluxConstants.FIELD_MONITORING, data.isMonitoringMode())
                    .addField(InfluxConstants.FIELD_SCORE, data.getScore())
                    .time(data.getTime().toInstant(), WRITE_PRECISION);
            writeApi.writePoint(usedBucket, InfluxConstants.ORG, point);
        });
    }
    public void saveHabits(List<BodyTrackingHabit> trackingHabits)
    {
        ThrowIfQueryUnavailable();

        WriteApiBlocking writeApi = influxClient.getWriteApiBlocking();
        String measurement = InfluxConstants.MEASUREMENT_HABIT;
        trackingHabits.forEach(data ->
        {
            System.out.println("Saving...: "+data.toString());
            // write by point
            Point point = Point.measurement(measurement)
                    .addTag(InfluxConstants.TAG_USER_ID, data.getUserId())
                    .addField(InfluxConstants.FIELD_HABIT, data.getPosturalAbnormalityCode())
                    .time(data.getTime().toInstant(), WRITE_PRECISION);
            writeApi.writePoint(usedBucket, InfluxConstants.ORG, point);
        });
    }

    private void ThrowIfQueryUnavailable()
    {
        if(isQueryUnavailable()) throw new RuntimeException("Could not access InfluxDB server...");
    }

    private Float objToFloat(Object value)
    {
        if(value == null)
            return 0f;
        if(value instanceof Double)
            return ((Double) value).floatValue();
        if(value instanceof Float)
            return (Float) value;
        return Float.valueOf(value.toString());
    }
    private boolean objToBoolean(Object value)
    {
        if(value == null)
            return false;
        if(value instanceof Boolean)
            return (Boolean) value;
        if(value instanceof String)
            return Boolean.parseBoolean(value.toString());
        return (boolean)value;
    }
}