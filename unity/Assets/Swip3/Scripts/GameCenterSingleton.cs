using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class GameCenterSingleton
{
	#region Singleton variables and functions
	private static GameCenterSingleton instance;
	
	public static GameCenterSingleton Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameCenterSingleton();
				instance.Initialize();
			}
			return instance;
		}
	}
	#endregion
	
	private IAchievement[] achievements;
	
	public GameCenterSingleton (){}
	
	public void Initialize()
	{
		if(!IsUserAuthenticated())
		{
			Social.localUser.Authenticate (ProcessAuthentication);
		}
	}    
	public bool IsUserAuthenticated()
	{
		if(Social.localUser.authenticated)
		{
			return true;
		}
		else
		{
			Debug.Log("User not Authenticated");
			return false;
		}
	}    
	public void ShowAchievementUI()
	{
		if(IsUserAuthenticated())
		{
			Social.ShowAchievementsUI();
		}
	}
	public void ShowLeaderboardUI()
	{
		if(IsUserAuthenticated())
		{
			//Social.ShowLeaderboardUI();
			GameCenterPlatform.ShowLeaderboardUI("com.unit9.swip3wear.leaderboard",TimeScope.AllTime);
		}
	}
	
	public void ReportScore(long score, string leaderBoardID)
	{
		if (Social.localUser.authenticated)
		{
			Debug.Log("Reporting score " + score + " on leaderboard " + leaderBoardID);
			Social.ReportScore(score, leaderBoardID, success =>
			                   {
				Debug.Log(success ? "Reported score successfully" : "Failed to report score");
			});
		}
	}
	public bool AddAchievementProgress(string achievementID, float percentageToAdd)
	{
		IAchievement a = GetAchievement(achievementID);
		if(a != null)
		{
			return ReportAchievementProgress(achievementID, ((float)a.percentCompleted + percentageToAdd));
		}
		else
		{
			return ReportAchievementProgress(achievementID, percentageToAdd);
		}
	}    
	public bool ReportAchievementProgress(string achievementID, float progressCompleted)
	{
		if(Social.localUser.authenticated)
		{
			if(!IsAchievementComplete(achievementID))
			{
				bool success = false;
				Social.ReportProgress(achievementID, progressCompleted, result => 
				                      {
					if (result)
					{
						success = true;
						LoadAchievements();
						Debug.Log ("Successfully reported progress");
					}
					else
					{
						success = false;
						Debug.Log ("Failed to report progress");
					}
				});
				
				return success;
			}
			else
			{
				return true;    
			}
		}
		else
		{
			Debug.Log("ERROR: GameCenter user not authenticated");
			return false;
		}
	}
	public void ResetAchievements()
	{
		GameCenterPlatform.ResetAllAchievements(ResetAchievementsHandler);    
	}
	
	void LoadAchievements()
	{
		Social.LoadAchievements (ProcessLoadedAchievements);
	}
	void ProcessAuthentication(bool success)
	{
		if(success)
		{
			Debug.Log ("Authenticated, checking achievements");
			
			LoadAchievements();
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);    
		}
		else
		{
			Debug.Log ("Failed to authenticate");
		}
	}    
	void ProcessLoadedAchievements (IAchievement[] achievements) 
	{
		//Clear the list
		if(this.achievements != null)
		{
			this.achievements = null;    
		}
		
		if (achievements.Length == 0)
		{
			Debug.Log ("Error: no achievements found");
		}
		else
		{
			Debug.Log ("Got " + achievements.Length + " achievements");
			this.achievements = achievements;
		}
	}
	bool IsAchievementComplete(string achievementID)
	{
		if(achievements != null)
		{
			foreach(IAchievement a in achievements)
			{
				if(a.id == achievementID && a.completed)
				{
					return true;    
				}
			}
		}
		
		return false;
	}
	IAchievement GetAchievement(string achievementID)
	{
		if(achievements != null)
		{
			foreach(IAchievement a in achievements)
			{
				if(a.id == achievementID)
				{
					return a;    
				}
			}
		}
		return null;
	}
	void ResetAchievementsHandler(bool status)
	{
		if(status)
		{
			//Clear the list
			if(this.achievements != null)
			{
				this.achievements = null;    
			}
			
			LoadAchievements();
			
			Debug.Log("Achievements successfully resetted!");
		}
		else
		{
			Debug.Log("Achievements reset failure!");
		}
	}
}