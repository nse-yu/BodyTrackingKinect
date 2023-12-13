package com.example.BodyTrackingServer.model;

import com.influxdb.annotations.Column;
import com.influxdb.annotations.Measurement;

import java.time.Instant;

@Measurement(name = "joint")
public class BodyTrackingData {
    @Column(tag = true)
    private String userId;
    @Column(tag = true)
    private String model;
    @Column
    private float positionX;
    @Column
    private float positionY;
    @Column
    private float positionZ;
    @Column(timestamp = true)
    private Instant time;

    public BodyTrackingData(String userId, String model, float positionX, float positionY, float positionZ, Instant time)
    {
        this.userId = userId;
        this.model = model;
        this.positionX = positionX;
        this.positionY = positionY;
        this.positionZ = positionZ;
        this.time = time;
    }

    public String getUserId() {
        return userId;
    }

    public void setUserId(String userId) {
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

    public Instant getTime() {
        return time;
    }

    public void setTime(Instant time) {
        this.time = time;
    }

    public float getPositionX() {
        return positionX;
    }

    public void setPositionX(float positionX) {
        this.positionX = positionX;
    }

    public float getPositionY() {
        return positionY;
    }

    public void setPositionY(float positionY) {
        this.positionY = positionY;
    }

    public float getPositionZ() {
        return positionZ;
    }

    public void setPositionZ(float positionZ) {
        this.positionZ = positionZ;
    }
}
