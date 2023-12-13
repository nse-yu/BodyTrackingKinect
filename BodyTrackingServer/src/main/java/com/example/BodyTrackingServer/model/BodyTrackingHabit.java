package com.example.BodyTrackingServer.model;

import com.example.BodyTrackingServer.constants.InfluxConstants;
import com.influxdb.annotations.Column;
import com.influxdb.annotations.Measurement;

import java.time.ZonedDateTime;

@Measurement(name = InfluxConstants.MEASUREMENT_HABIT)
public class BodyTrackingHabit
{
    @Column(tag = true)
    private String userId;
    @Column
    private String posturalAbnormalityCode;
    @Column(timestamp = true)
    private ZonedDateTime time;

    public BodyTrackingHabit(String userId, String posturalAbnormalityCode, ZonedDateTime time)
    {
        this.userId = userId;
        this.posturalAbnormalityCode = posturalAbnormalityCode;
        this.time = time;
    }

    public String getUserId()
    {
        return userId;
    }
    public String getPosturalAbnormalityCode()
    {
        return posturalAbnormalityCode;
    }
    public ZonedDateTime getTime()
    {
        return time;
    }

    @Override
    public String toString()
    {
        return "BodyTrackingHabit{" +
                "userId='" + userId + '\'' +
                ", posturalAbnormalityCode='" + posturalAbnormalityCode + '\'' +
                ", time=" + time +
                '}';
    }
}
