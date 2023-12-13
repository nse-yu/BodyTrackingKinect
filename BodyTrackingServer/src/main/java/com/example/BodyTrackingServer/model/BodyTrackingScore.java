package com.example.BodyTrackingServer.model;

import com.example.BodyTrackingServer.constants.InfluxConstants;
import com.influxdb.annotations.Column;
import com.influxdb.annotations.Measurement;

import java.time.ZonedDateTime;

@Measurement(name = InfluxConstants.MEASUREMENT_SCORE)
public class BodyTrackingScore
{
    @Column(tag = true)
    private String userId;
    @Column(tag = true)
    private String model;
    @Column
    private float score;
    @Column(tag = true)
    private boolean monitoringMode;
    @Column(timestamp = true)
    private ZonedDateTime time;

    public BodyTrackingScore(String userId, String model, float score, boolean monitoringMode, ZonedDateTime time)
    {
        this.userId = userId;
        this.model = model;
        this.score = score;
        this.monitoringMode = monitoringMode;
        this.time = time;
    }

    public String getUserId()
    {
        return userId;
    }

    public void setUserId(String userId)
    {
        this.userId = userId;
    }

    public String getModel()
    {
        return model;
    }

    public void setModel(String model)
    {
        this.model = model;
    }

    public float getScore()
    {
        return score;
    }

    public void setScore(float score)
    {
        this.score = score;
    }

    public boolean isMonitoringMode()
    {
        return monitoringMode;
    }

    public void setMonitoringMode(boolean monitoringMode)
    {
        this.monitoringMode = monitoringMode;
    }

    public ZonedDateTime getTime()
    {
        return time;
    }

    public void setTime(ZonedDateTime time)
    {
        this.time = time;
    }

    @Override
    public String toString()
    {
        return "BodyTrackingScore{" +
                "userId='" + userId + '\'' +
                ", model='" + model + '\'' +
                ", score=" + score +
                ", monitoringMode=" + monitoringMode +
                ", time=" + time +
                '}';
    }
}
