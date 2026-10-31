using TutoringCentre.Evaluation.Runner;
if (args is ["journey-snapshot"]) return await JourneySnapshotCommand.ExecuteAsync();
return await RunCommand.ExecuteAsync(args);
