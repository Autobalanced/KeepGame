using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job {

    // This class holds information for a queued up job such as building or repairing a building.

    // What tile the job is located in
    public Tile tile { get; protected set; }

    // Time it takes to complete the job
    float jobTime;

    // When job completes or is cancelled run this callback
    Action<Job> cbJobComplete;
    Action<Job> cbJobCancelled;

    public Job (Tile tile, Action<Job> cbJobComplete, float jobTime = 1f)
    {
        this.tile = tile;
        this.cbJobComplete += cbJobComplete;
    }

    public void RegisterJobCompleteCallback(Action<Job> cb)
    {
        cbJobComplete += cb;
    }

    public void RegisterJobCompleteCancelled(Action<Job> cb)
    {
        cbJobCancelled += cb;
    }

    // Put in the necessary time to complete the job
    public void DoJob(float workTime)
    {
        jobTime -= workTime;

        if (jobTime <= 0)
        {
            if (cbJobComplete != null)
            {
                cbJobComplete(this);
            }
        }
    }

    public void CancelJob()
    {
        if (cbJobCancelled != null)
        {
            cbJobCancelled(this);
        }
    }
}
