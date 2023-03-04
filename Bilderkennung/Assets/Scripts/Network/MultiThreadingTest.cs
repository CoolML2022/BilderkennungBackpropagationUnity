using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
public class MultiThreadingTest : MonoBehaviour
{
    [SerializeField] private bool useJobs;
    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (useJobs)
        {
            NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 40; i++)
            {
                JobHandle jobHandle = ReallyToughTaskJob();
                jobHandles.Add(jobHandle);
            }
            JobHandle.CompleteAll(jobHandles);
            jobHandles.Dispose();
        }
        else
        {
            for (int i = 0; i < 20; i++)
            {
                ToughTask();
            }

        }    
        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f +  "ms");
    }
    private void ToughTask()
    {
        float value = 0f;
        for(int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
            value = math.log2(value);
        }
    }
    private JobHandle ReallyToughTaskJob()
    {
        ToughTask job = new ToughTask();
        return job.Schedule();
    }
}
[BurstCompile]
public struct ToughTask : IJob
{
    public void Execute()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
            value = math.log2(value);
        }
    }
}