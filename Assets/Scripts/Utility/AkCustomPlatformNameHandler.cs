public partial class AkBasePathGetter
{
	static partial void GetCustomPlatformName(ref string platformName)
	{
#if UNITY_ANDROID
		platformName = "Android_High";
#endif
	}

}
