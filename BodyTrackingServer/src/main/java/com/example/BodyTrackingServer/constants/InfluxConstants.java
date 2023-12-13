package com.example.BodyTrackingServer.constants;

import com.influxdb.client.domain.WritePrecision;

import java.time.ZoneId;

public class InfluxConstants
{
    public static final String URI_BUCKET = "body-tracking-store";
    public static final String MEASUREMENT_SCORE = "joint";
    public static final String MEASUREMENT_HABIT = "habit";
    public static final String ORG = "student";
    public static final String TAG_USER_ID = "userid";
    public static final String TAG_MODEL = "model";
    public static final String FIELD_MONITORING = "monitoringMode";
    public static final String FIELD_SCORE = "score";
    public static final String FIELD_HABIT = "habit";
    public static final ZoneId TIME_ZONE = ZoneId.of("Asia/Tokyo");

}
