package com.example.BodyTrackingServer.model;

import com.example.BodyTrackingServer.constants.InfluxConstants;
import org.springframework.boot.context.properties.ConfigurationProperties;

@ConfigurationProperties("spring.influx")
public class InfluxDBConfig {
    public String url = "";
    public String database = "";
    public String username = "root";
    public String password = "root";
    public String bucket = "";
    public String token = "";
    public String org = InfluxConstants.ORG;

    public void setUrl(String url) {
        this.url = url;
    }

    public void setDatabase(String database) {
        this.database = database;
    }

    public void setUsername(String username) {
        this.username = username;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public void setToken(String token)
    {
        this.token = token;
    }

    public void setBucket(String bucket)
    {
        this.bucket = bucket;
    }

    public void setOrg(String org)
    {
        this.org = org;
    }

    @Override
    public String toString()
    {
        return "InfluxDBConfig{" +
                "url='" + url + '\'' +
                ", database='" + database + '\'' +
                ", username='" + username + '\'' +
                ", password='" + password + '\'' +
                ", bucket='" + bucket + '\'' +
                ", token='" + token + '\'' +
                ", org='" + org + '\'' +
                '}';
    }
}
