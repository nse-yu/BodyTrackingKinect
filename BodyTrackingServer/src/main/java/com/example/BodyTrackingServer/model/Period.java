package com.example.BodyTrackingServer.model;

public class Period
{
    private long start;
    private long stop;

    public Period(long start, long stop)
    {
        this.start = start;
        this.stop = stop;
    }

    public long getStart()
    {
        return start;
    }

    public void setStart(long start)
    {
        this.start = start;
    }

    public long getStop()
    {
        return stop;
    }

    public void setStop(long stop)
    {
        this.stop = stop;
    }
}
