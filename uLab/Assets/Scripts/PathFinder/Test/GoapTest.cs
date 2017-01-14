using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Lite;
using Lite.Strategy;
using Lite.Goap;
using Lite.Knowledge;


public class GoapTest : MonoBehaviour
{
	AppFacade app;
	Agent blacksmith;
	Agent logger;
	Agent miner;
	Agent woodCutter;

	void Start()
	{
		app = AppFacade.Instance;
		app.Init();

		blacksmith = new Agent(GuidGenerator.NextLong(), Career.Blacksmith);
		blacksmith.name = "Blacksmith";
		app.stgAgentManager.AddAgent(blacksmith);

		logger = new Agent(GuidGenerator.NextLong(), Career.Logger);
		logger.name = "logger";
		app.stgAgentManager.AddAgent(logger);

		miner = new Agent(GuidGenerator.NextLong(), Career.Miner);
		miner.name = "miner";
		app.stgAgentManager.AddAgent(miner);

		woodCutter = new Agent(GuidGenerator.NextLong(), Career.WoodCutter);
		woodCutter.name = "woodCutter";
		app.stgAgentManager.AddAgent(woodCutter);

		// goals
		blacksmith.AddGoal(new Goal_MakeTools());
		logger.AddGoal(new Goal_MakeLogs());
		miner.AddGoal(new Goal_MakeOre());
		woodCutter.AddGoal(new Goal_MakeFirewood());
	}

	void Update()
	{
		app.Update();
	}

	/*long mills = 0;
	void OnGUI()
	{
		
		if (GUI.Button(new Rect(10, 10, 40, 20), "T"))
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			path = pathFinder.Plan(null);
			watch.Stop();
			mills = watch.ElapsedMilliseconds;
		}
		GUI.Label(new Rect(50, 0, 100, 30), "ms " + mills);

	}*/

}