Unfortunately NuGet package 2.0.4 don't work on Xamarin. 
Here are the steps I took to make it run:
- Clone the source code, commit: 381b72c48a9929c55e3469d2bd50b3176a6d5f89
- Removed net48 target from project file
- In HarmonySharedState cs.file line 16, comment name parameter from Mutex (throw exception).
