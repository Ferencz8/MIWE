# MIWE
Work In Progress

**M**ultiple **I**nstance **W**ork **E**xecuter is capable of running different crawling jobs, in the form of dlls. These jobs have a common contract/interface used by the Executer to identify, load dynamically using reflection and start. Once the data is extracted from the HTML pages, by the crawlers, they are processed dynamically by data processing jobs.
The execution of the crawling & data processing plugins can be scheduled using Quartz CRON expressions.


Once launched the Work Executer registers it’s IP in an MS-SQL database.
Multiple Instances can be launched, which can interact and load/offload work on each other using **gRPC communication**
The main Instance which is launched will register as the "Master" Instance, if no other "Master" instance is available. The next instances will register as "Slave".
The Master instance runs and/or distributes the workload among registered and available Slave instances.
The Slave instance receives work to execute and constantly checks to see if the Master is available to take it's place in case it isn't.

## High Level Architecture
![High Level Architecture](https://github.com/Ferencz8/MIWE/blob/main/Diagrams/workerDiagram.png)

## Platform Presentation

### Jobs Listing
![Jobs Listing](https://github.com/Ferencz8/MIWE/blob/main/Diagrams/jobs.png)

### Add a Job Plugin
![Add a Job Plugin](https://github.com/Ferencz8/MIWE/blob/main/Diagrams/addJob.PNG)

### Edit a Job Scheduling
![Edit a Job Scheduling](https://github.com/Ferencz8/MIWE/blob/main/Diagrams/editJob.PNG)

### Result of a Job Session
![Result of Job Sessions](https://github.com/Ferencz8/MIWE/blob/main/Diagrams/sessions.PNG)
