#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml.Serialization;

namespace xARM{
	
	public enum xARMScreenCapGroup {iOS=0, Android, WinPhone8, WindowsRT, Standalone=10, Custom};
	
	public enum EditorMode {Other, Edit, Pause, Play};
	
	public class xARMScreenCap {

		#region Fields
		public static bool ListChanged = false;

		public string Name;
		public string LongName;
		public string Description;
		public Vector2 Resolution;
		public string ResolutionLabel;
		public float Aspect;
		public string AspectLabel;
		public float Diagonal;
		public int DPI;
		public string DPILabel;
		public float DPIFactor;
		public xARMScreenCapGroup Group;
		public int StatisticsPositioning;
		public float StatisticsUsedPercent;
		private Texture2D _texture;
		private bool enabled;
		public bool IsLandscape;
		public bool IsPortrait;
		public bool IsBaseRes;
		[XmlIgnore]
		public double LastUpdateTime = 0;
		[XmlIgnore]
		public double LastUpdateTryTime = 0;
		[XmlIgnore]
		public bool UpdatedSuccessful = true;
		
		[XmlIgnore]
		private static char charInch = '\u2033';
		#endregion
		
		#region Properties
		[XmlIgnore]
		public Texture2D Texture{
			get {
				if(_texture == null) _texture = EditorGUIUtility.whiteTexture;
				return _texture;
				}
			set {_texture = value;}
		}

		public bool Enabled {
			get {
				return enabled;
			}
			set {
				// mark SC list as changed to enable config save on change
				if (enabled != value){
					ListChanged = true;
				}

				enabled = value;
			}
		}
		
		public bool OrientationIsActive{
			get {
				if((this.IsLandscape && xARMManager.Config.ShowLandscape && (this.IsBaseRes || !this.IsBaseRes && xARMManager.Config.ShowNavigationBar)) || 
				   (this.IsPortrait && xARMManager.Config.ShowPortrait && (this.IsBaseRes || !this.IsBaseRes && xARMManager.Config.ShowNavigationBar)))
				{
					return true;
				} else {
					return false;
				}
			}
		}
		
		#endregion
		
		#region Init
		public xARMScreenCap (){}
		
		public xARMScreenCap (string name, Vector2 res, string resLabel, string aspectLable, float diagonal, int dpi, string dpiLabel, 
			xARMScreenCapGroup scgroup, int statsPos, float usedPerc, string desc){
			
			if(name != ""){
				Name = name;
			} else {
				Name = scgroup.ToString ();
			}
			
			Description = desc;
			Resolution = res;
			ResolutionLabel = resLabel;
			Aspect = res.x / res.y;
			
			if(aspectLable != ""){
				AspectLabel = aspectLable;
			} else {
				if(Aspect.Equals (3/2f)){
					AspectLabel = "3:2";
				} else if(Aspect.Equals (5/3f)){
					AspectLabel = "5:3";
				} else if(Aspect.Equals (5/4f)){
					AspectLabel = "5:4";
				} else if(Aspect.Equals (4/3f)){
					AspectLabel = "4:3";
				} else if(Aspect.Equals (16/9f)){
					AspectLabel = "16:9";
				} else if(Aspect.Equals (16/10f)){
					AspectLabel = "16:10";
				} else {
					AspectLabel = "custom";
				}
			}
			
			Diagonal = diagonal;
			if(dpi == 0){
				DPI = Mathf.RoundToInt (Mathf.Sqrt (Mathf.Pow(res.x, 2) + Mathf.Pow(res.y, 2)) / Diagonal);
			} else{
				DPI = dpi;
			}
			
			if(dpiLabel != ""){
				DPILabel = dpiLabel;
			} else {
				if(scgroup == xARMScreenCapGroup.Android){
					if(DPI<140){				
						DPILabel = "ldpi";
					} else if(DPI<200){
						DPILabel = "mdpi";
					} else if(DPI > 210 && DPI < 220){
						DPILabel = "tvdpi"; // 213
					} else if(DPI<280){
						DPILabel = "hdpi";
					} else if(DPI<400){
						DPILabel = "xhdpi";
					} else if (DPI<560){
						DPILabel = "xxhdpi";
					} else {
						DPILabel = "xxxhdpi";
					}
					
				}
			}
			
			switch (DPILabel){
				case "ldpi":
					DPIFactor = 0.7f;
					break;
				case "mdpi":
					DPIFactor = 1f;
					break;
				case "tvdpi":
					DPIFactor = 1.33f;
					break;
				case "hdpi":
					DPIFactor = 1.5f;
					break;
				case "xhdpi":
					DPIFactor = 2f;
					break;
				case "xxhdpi":
					DPIFactor = 3f;
					break;
				case "xxxhdpi":
					DPIFactor = 4f;
					break;
			}
			
			Group = scgroup;
			StatisticsPositioning = statsPos;
			StatisticsUsedPercent = usedPerc;
			enabled = false;
			
			if(res.x > res.y){
				IsLandscape = true;
			} else {
				IsLandscape = false;
			}
			
			IsPortrait = !IsLandscape;
			IsBaseRes = true;

			LongName = Name + " " + Diagonal + charInch+ " " + DPILabel + " (" + AspectLabel + ") " + 
					Resolution.x + "x" + Resolution.y + "px (" + ResolutionLabel + ")";
		}
		#endregion
		
		#region Functions
		public xARMScreenCap Clone(bool switchResolutionXY = false){
			xARMScreenCap newScreenCap;
			
			if(switchResolutionXY){
				newScreenCap = new xARMScreenCap(this.Name, new Vector2(this.Resolution.y, this.Resolution.x), this.ResolutionLabel, this.AspectLabel, this.Diagonal, 
					this.DPI, this.DPILabel, this.Group, this.StatisticsPositioning, this.StatisticsUsedPercent, this.Description);
				
			} else{
				newScreenCap = new xARMScreenCap(this.Name, this.Resolution, this.ResolutionLabel, this.AspectLabel, this.Diagonal, 
					this.DPI, this.DPILabel, this.Group, this.StatisticsPositioning, this.StatisticsUsedPercent, this.Description);
			}
			
			return newScreenCap;
		}
		
		
		public xARMScreenCap[] CreateNavigationBarVersion(){
//			xARMScreenCap newScreenCap;
			int MDPINavigationBarHeight = 48;
			int navigationBarHeight = Mathf.RoundToInt (MDPINavigationBarHeight * this.DPIFactor);
			Vector2 navBarRes, navBarResToReplace;

			// on smartphones the navigation bar is always at the same spot
			if (this.IsLandscape && this.Diagonal <= 6.5f){
				navBarRes = new Vector2(this.Resolution.x - navigationBarHeight, this.Resolution.y);
				navBarResToReplace = new Vector2(this.Resolution.x, this.Resolution.y - navigationBarHeight); // incorrect resolution

			} else { // NavBar is allways at the bottom
				navBarRes = new Vector2(this.Resolution.x, this.Resolution.y - navigationBarHeight);
				navBarResToReplace = navBarRes; // keep all values
			}

			xARMScreenCap newScreenCap = new xARMScreenCap(this.Name, navBarRes, 
				this.ResolutionLabel + "~", this.AspectLabel, this.Diagonal, this.DPI, this.DPILabel, this.Group, this.StatisticsPositioning, 
				this.StatisticsUsedPercent, this.Description + " (navigation bar version)");
			newScreenCap.IsBaseRes = false;

			xARMScreenCap screenCapToReplace = new xARMScreenCap(this.Name, navBarResToReplace, 
			    this.ResolutionLabel + "~", this.AspectLabel, this.Diagonal, this.DPI, this.DPILabel, this.Group, this.StatisticsPositioning, 
			    this.StatisticsUsedPercent, this.Description + " (navigation bar version)");

			return new xARMScreenCap[] {newScreenCap, screenCapToReplace};
		}
		#endregion 
		
		#region Operators
		public static bool operator == (xARMScreenCap a, xARMScreenCap b){
			if(a.Resolution == b.Resolution && a.Diagonal == b.Diagonal && a.Group == b.Group){
				return true;
			} else {
				return false;
			}
		}
		
		public static bool operator != (xARMScreenCap a, xARMScreenCap b){
			return !(a == b);	
		}
		
		public override bool Equals (object otherObject){
			if(!(otherObject is xARMScreenCap)) return false;
			return this == (xARMScreenCap)otherObject;	
		}
		
		public override int GetHashCode(){
			unchecked{
				int hash = 17;
				hash = hash * 23 + Resolution.GetHashCode();
				hash = hash * 23 + Diagonal.GetHashCode();
				hash = hash * 23 + Group.GetHashCode();
				return hash;
			}
		}
		#endregion
	}
}
#endif