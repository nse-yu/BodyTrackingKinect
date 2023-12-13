package com.example.BodyTrackingServer.database;

import com.example.BodyTrackingServer.constants.InfluxConstants;
import com.example.BodyTrackingServer.model.BodyTrackingHabit;
import com.example.BodyTrackingServer.model.BodyTrackingScore;
import com.example.BodyTrackingServer.model.Period;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

import java.util.ArrayList;
import java.util.List;

/** 例外処理について、、、
 * 基本的に、このクラスからは例外を出さないようにする。
 * InfluxDAOは例外を投げてくるため、ここで吸収する必要がある。
 * */
@Repository
public class BodyTrackingRepository
{
    @Autowired
    InfluxDao influxDao;

    public List<BodyTrackingScore> getScores(String bucket, long past)
    {
        return getScores(bucket, "", past);
    }
    public List<BodyTrackingScore> getScores(String bucket, String model, long past)
    {
        return getScores(bucket, model, new Period(past, 0));
    }
    public List<BodyTrackingScore> getScores(String bucket, String model, Period period)
    {
        if(period == null) return new ArrayList<>();

        List<BodyTrackingScore> bodyTrackingScores = null;
        try
        {
            bodyTrackingScores = influxDao
                    .setBucket(bucket.isEmpty() ? InfluxConstants.URI_BUCKET : bucket)
                    .getScores(model, period.getStart(), period.getStop());
        }catch(Exception e)
        {
            InfluxDao.log.error("Failed to get saved scores...:\r\n"+e.getMessage());
            return new ArrayList<>();
        }
        return bodyTrackingScores;
    }

    public List<BodyTrackingHabit> getHabits(String bucket, long past)
    {
        return getHabits(bucket, new Period(past, 0));
    }
    public List<BodyTrackingHabit> getHabits(String bucket, Period period)
    {
        if(period == null) return new ArrayList<>();

        List<BodyTrackingHabit> bodyTrackingHabits = null;
        try
        {
            bodyTrackingHabits = influxDao
                    .setBucket(bucket.isEmpty() ? InfluxConstants.URI_BUCKET : bucket)
                    .getHabits(period.getStart(), period.getStop());
        }catch(Exception e)
        {
            InfluxDao.log.error("Failed to get saved habits...:\r\n"+e.getMessage());
            return new ArrayList<>();
        }
        return bodyTrackingHabits;
    }

    public boolean saveScores(String bucket, List<BodyTrackingScore> trackingList)
    {
        try
        {
            influxDao
                    .setBucket(bucket.isEmpty() ? InfluxConstants.URI_BUCKET : bucket)
                    .saveScores(trackingList);
        }catch(Exception e)
        {
            InfluxDao.log.error("Failed to save tracking scores:\r\n" + e.getMessage());
            return false;
        }
        return true;
    }
    public boolean saveHabits(String bucket, List<BodyTrackingHabit> trackingList)
    {
        try
        {
            influxDao
                    .setBucket(bucket.isEmpty() ? InfluxConstants.URI_BUCKET : bucket)
                    .saveHabits(trackingList);
        }catch(Exception e)
        {
            InfluxDao.log.error("Failed to save tracking habits:\r\n" + e.getMessage());
            return false;
        }
        return true;
    }
    public List<String> findBucketAll()
    {
        try
        {
            return influxDao.findBucketAll();
        }
        catch(Exception e)
        {
            InfluxDao.log.error("Failed to read buckets from InfluxDB server, please checking configuration files.");
        }
        return new ArrayList<>();
    }
}
