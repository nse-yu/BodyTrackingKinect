package com.example.BodyTrackingServer;

import com.example.BodyTrackingServer.constants.InfluxConstants;
import com.example.BodyTrackingServer.database.BodyTrackingRepository;
import com.example.BodyTrackingServer.model.BodyTrackingHabit;
import com.example.BodyTrackingServer.model.BodyTrackingScore;
import com.example.BodyTrackingServer.model.Period;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

//TODO: influx delete --bucket body-tracking-store --start 2023-06-26T00:00:00Z --stop 2023-06-27T00:00:00Z
/*
* 1. Group Key - 全rowが共通して持つcolumnのこと。_timeや_value,_measurement,もしくは自作のtagなど
* 2. tableはgroupごとに生成されるため、初期状態では、レコードの数だけテーブルができる。
* 3. pivotは、同一時間の_valueを結合することができる。例として、異なるレコードが同一時刻に3個づつ、１分おきに送信されるとして、
*    3分後には9個のテーブルもしくはレコードが存在する。
*    なので、pivotで結合すると9÷3=3個のテーブルもしくはレコードに圧縮できる。（厳密には、1テーブルに1レコードという）
*    だが、プログラム上で実行すると、得られるデータ構造は、テーブル3つ、それぞれにレコード3つとなり、各テーブルのレコードはどれも同じ
*    値になってしまう。
* 4. aggregateWindowは、rangeの示す期間をeveryの時間で分割し、別のレコードグループに分け、各グループに集約関数を適用する。
*    テーブル数はGroup Keyを基準としているため、3つの属性を持つtagがkeyの場合、3つのテーブルの各期間において集約される。
* 5. Retention Policyみたいなので、日付的に比較的過去のものはデータベースに格納できない。(6/28日に6/3日のデータは格納不可を確認)
* */
@SuppressWarnings("unused")
@RestController
@RequestMapping("/body-tracking")
public class BodyTrackingController
{
    @Autowired
    private BodyTrackingRepository bodyTrackingRepository;

    @GetMapping(path = "/hello")
    public String Hello()
    {
        return "hello";
    }

    @GetMapping(path = "/joint/get")
    public List<BodyTrackingScore> getTrackingScores(
        @RequestParam String model,
        @RequestParam long start,
        @RequestParam long stop)
    {
        return bodyTrackingRepository.getScores(InfluxConstants.URI_BUCKET, model, new Period(start, stop));
    }
    @GetMapping(path = "/joint/get/{past}")
    public List<BodyTrackingScore> getTrackingScores(@PathVariable long past)
    {
        return bodyTrackingRepository.getScores(InfluxConstants.URI_BUCKET, past);
    }
    @GetMapping(path = "/joint/get/{past}/{model}")
    public List<BodyTrackingScore> getTrackingScores(
            @PathVariable String model,
            @PathVariable long past
    )
    {
        return bodyTrackingRepository.getScores(InfluxConstants.URI_BUCKET, model, past);
    }
    @GetMapping(path = "/habit/get")
    public List<BodyTrackingHabit> getTrackingHabits(
            @RequestParam long start,
            @RequestParam long stop
    )
    {
        return bodyTrackingRepository.getHabits(InfluxConstants.URI_BUCKET, new Period(start, stop));
    }
    @GetMapping(path = "/habit/get/{past}")
    public List<BodyTrackingHabit> getTrackingHabits(@PathVariable long past)

    {
        return bodyTrackingRepository.getHabits(InfluxConstants.URI_BUCKET, past);
    }
    @PostMapping(path = "/joint/save")
    public ResponseEntity<String> saveTrackingScores(@RequestBody List<BodyTrackingScore> trackingList)
    {
        if(bodyTrackingRepository.saveScores(InfluxConstants.URI_BUCKET, trackingList))
            return new ResponseEntity<>("The body tracking scores has been saved successfully.", HttpStatus.CREATED);
        return new ResponseEntity<>("Due to some problems, the body tracking scores could not be saved...", HttpStatus.EXPECTATION_FAILED);
    }
    @PostMapping(path = "/habit/save")
    public ResponseEntity<String> saveHabitHistories(@RequestBody List<BodyTrackingHabit> trackingList)
    {
        if(bodyTrackingRepository.saveHabits(InfluxConstants.URI_BUCKET, trackingList))
            return new ResponseEntity<>("The body tracking habits has been saved successfully.", HttpStatus.CREATED);
        return new ResponseEntity<>("Due to some problems, the body tracking habits could not be saved...", HttpStatus.EXPECTATION_FAILED);
    }
    @GetMapping(path = "/buckets")
    public List<String> showBuckets()
    {
        return bodyTrackingRepository.findBucketAll();
    }
}
