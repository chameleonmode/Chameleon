using Chameleon.Interfaces.Dialogs;

namespace Chameleon.Common.Icons;

public class FontIconInfo : IFontIconInfo
{
    public FontIconInfo(string name, string codepoint)
    {
        Name = name;
        Codepoint = codepoint;
        Glyph = char.ConvertFromUtf32((int)Convert.ToUInt32(codepoint, 16)).ToString();
    }

    public string Name { get; set; }

    public string Codepoint { get; set; }

    public string Glyph { get; }
}

public static class FontIcons
{
    public static IFontIconInfo? Filter(string name)
    {
        return FontIconsInfos.FirstOrDefault(i => i.Name == name); 
    }

    public static List<FontIconInfo> FontIconsInfos =
         [
    new ("Accept",
         "E10B"
  ),
  
    new ("Account",
    "E168"
  ),
  
    new ("Add",
    "E109"
  ),
  
    new ("AddFriend",
    "E1E2"
  ),
  
    new ("Admin",
    "E1A7"
  ),
  
    new ("Alert",
    "F8001"
  ),
  
    new ("AlertFilled",
    "F8002"
  ),
  
    new ("AlertOff",
    "F8003"
  ),
  
    new ("AlertOffFilled",
    "F8004"
  ),
  
    new ("AlertOn",
    "F8005"
  ),
  
    new ("AlertOnFilled",
    "F8006"
  ),
  
    new ("AlertSnooze",
    "F8007"
  ),
  
    new ("AlertSnoozeFilled",
    "F8008"
  ),
  
    new ("AlertUrgent",
    "F8009"
  ),
  
    new ("AlertUrgentFilled",
    "F800A"
  ),
  
    new ("AlignCenter",
    "E1A1"
  ),
  
    new ("AlignDistributed",
    "F800C"
  ),
  
    new ("AlignJustified",
    "F800E"
  ),
  
    new ("AlignLeft",
    "E1A2"
  ),
  
    new ("AlignRight",
    "E1A0"
  ),
  
    new ("AllApps",
    "E179"
  ),
  
    new ("Attach",
    "E16C"
  ),
  
    new ("AttachCamera",
    "E12D"
  ),
  
    new ("Audio",
    "E189"
  ),
  
    new ("Back",
    "E112"
  ),
  
    new ("BackToWindow",
    "E1D8"
  ),
  
    new ("BlockContact",
    "E1E0"
  ),
  
    new ("Bold",
    "E19B"
  ),
  
    new ("Bookmark",
    "F8015"
  ),
  
    new ("BookmarkFilled",
    "F8016"
  ),
  
    new ("Bookmarks",
    "E12F"
  ),
  
    new ("BrowsePhotos",
    "E155"
  ),
  
    new ("BulletList",
    "F8017"
  ),
  
    new ("Bullets",
    "E133"
  ),
  
    new ("Calculator",
    "E1D0"
  ),
  
    new ("CalculatorFilled",
    "F8019"
  ),
  
    new ("Calendar",
    "E163"
  ),
  
    new ("CalendarDay",
    "E161"
  ),
  
    new ("CalendarDayFilled",
    "F801A"
  ),
  
    new ("CalendarEmpty",
    "F801B"
  ),
  
    new ("CalendarEmptyFilled",
    "F801C"
  ),
  
    new ("CalendarFilled",
    "F801D"
  ),
  
    new ("CalendarMonth",
    "F801E"
  ),
  
    new ("CalendarMonthFilled",
    "F801F"
  ),
  
    new ("CalendarReply",
    "E1DB"
  ),
  
    new ("CalendarReplyFilled",
    "F8020"
  ),
  
    new ("CalendarSync",
    "F8021"
  ),
  
    new ("CalendarSyncFilled",
    "F8022"
  ),
  
    new ("CalendarToday",
    "F8023"
  ),
  
    new ("CalendarTodayFilled",
    "F8024"
  ),
  
    new ("CalendarWeek",
    "E162"
  ),
  
    new ("Camera",
    "E114"
  ),
  
    new ("CameraFilled",
    "F8025"
  ),
  
    new ("Cancel",
    "E10A"
  ),
  
    new ("CellPhone",
    "E1C9"
  ),
  
    new ("Character",
    "E164"
  ),
  
    new ("Checkmark",
    "E73E"
  ),
  
    new ("ChevronDown",
    "E70D"
  ),
  
    new ("ChevronLeft",
    "E76B"
  ),
  
    new ("ChevronRight",
    "E76C"
  ),
  
    new ("ChevronUp",
    "E70E"
  ),
  
    new ("Clear",
    "E106"
  ),
  
    new ("ClearFormatting",
    "F8030"
  ),
  
    new ("ClearFormattingFilled",
    "F8031"
  ),
  
    new ("ClearSelection",
    "E1C5"
  ),
  
    new ("Clipboard",
    "F8032"
  ),
  
    new ("ClipboardCode",
    "F8033"
  ),
  
    new ("ClipboardCodeFilled",
    "F8034"
  ),
  
    new ("ClipboardFilled",
    "F8035"
  ),
  
    new ("Clock",
    "E121"
  ),
  
    new ("ClockFilled",
    "F8036"
  ),
  
    new ("ClosedCaption",
    "E190"
  ),
  
    new ("ClosedCaptionFilled",
    "F8037"
  ),
  
    new ("Cloud",
    "E753"
  ),
  
    new ("CloudBackup",
    "F8039"
  ),
  
    new ("CloudBackupFilled",
    "F803A"
  ),
  
    new ("CloudDownload",
    "EBD3"
  ),
  
    new ("CloudDownloadFilled",
    "F803C"
  ),
  
    new ("CloudFilled",
    "F803D"
  ),
  
    new ("CloudOff",
    "F803E"
  ),
  
    new ("CloudOffFilled",
    "F803F"
  ),
  
    new ("CloudOffline",
    "F8040"
  ),
  
    new ("CloudOfflineFilled",
    "F8041"
  ),
  
    new ("CloudSync",
    "F8042"
  ),
  
    new ("CloudSyncComplete",
    "F8043"
  ),
  
    new ("CloudSyncCompleteFilled",
    "F8044"
  ),
  
    new ("CloudSyncFilled",
    "F8045"
  ),
  
    new ("Code",
    "E943"
  ),
  
    new ("CodeHTML",
    "F8134"
  ),
  
    new ("ColorBackground",
    "F8048"
  ),
  
    new ("ColorBackgroundFilled",
    "F8049"
  ),
  
    new ("ColorFill",
    "F804A"
  ),
  
    new ("ColorFillFilled",
    "F804B"
  ),
  
    new ("ColorLine",
    "F804C"
  ),
  
    new ("ColorLineFilled",
    "F804D"
  ),
  
    new ("Comment",
    "E134"
  ),
  
    new ("CommentAdd",
    "F804E"
  ),
  
    new ("CommentAddFilled",
    "F804F"
  ),
  
    new ("CommentFilled",
    "F8050"
  ),
  
    new ("CommentMention",
    "F8051"
  ),
  
    new ("CommentMentionFilled",
    "F8052"
  ),
  
    new ("CommentMultiple",
    "F8053"
  ),
  
    new ("CommentMultipleFilled",
    "F8054"
  ),
  
    new ("Contact",
    "E13D"
  ),
  
    new ("Contact2",
    "E187"
  ),
  
    new ("ContactInfo",
    "E136"
  ),
  
    new ("ContactInfoFilled",
    "F8055"
  ),
  
    new ("Copy",
    "E16F"
  ),
  
    new ("CopyFilled",
    "F8056"
  ),
  
    new ("Crop",
    "E123"
  ),
  
    new ("Cut",
    "E16B"
  ),
  
    new ("DarkTheme",
    "F8059"
  ),
  
    new ("Delete",
    "E107"
  ),
  
    new ("DeleteFilled",
    "F805B"
  ),
  
    new ("Directions",
    "E1D1"
  ),
  
    new ("DirectionsFilled",
    "F805C"
  ),
  
    new ("Dislike",
    "E19E"
  ),
  
    new ("DislikeFilled",
    "F805D"
  ),
  
    new ("Dismiss",
    "F805E"
  ),
  
    new ("DockBottom",
    "E147"
  ),
  
    new ("DockLeft",
    "E145"
  ),
  
    new ("DockLeftFilled",
    "F8060"
  ),
  
    new ("DockRight",
    "E146"
  ),
  
    new ("DockRightFilled",
    "F8061"
  ),
  
    new ("Document",
    "E130"
  ),
  
    new ("DocumentFilled",
    "F8062"
  ),
  
    new ("Download",
    "E118"
  ),
  
    new ("Earth",
    "F8064"
  ),
  
    new ("EarthFilled",
    "F8065"
  ),
  
    new ("Edit",
    "E104"
  ),
  
    new ("EditFilled",
    "F8066"
  ),
  
    new ("Emoji",
    "E11D"
  ),
  
    new ("Emoji2",
    "E170"
  ),
  
    new ("EmojiFilled",
    "F8067"
  ),
  
    new ("Favorite",
    "E113"
  ),
  
    new ("Filter",
    "E16E"
  ),
  
    new ("Find",
    "E11A"
  ),
  
    new ("Flag",
    "E129"
  ),
  
    new ("FlagFilled",
    "F8069"
  ),
  
    new ("Folder",
    "E188"
  ),
  
    new ("FolderFilled",
    "F806A"
  ),
  
    new ("FolderLink",
    "F806B"
  ),
  
    new ("FolderLinkFilled",
    "F806C"
  ),
  
    new ("Font",
    "E185"
  ),
  
    new ("FontColor",
    "E186"
  ),
  
    new ("FontColorFilled",
    "F806D"
  ),
  
    new ("FontDecrease",
    "E1C6"
  ),
  
    new ("FontIncrease",
    "E1C7"
  ),
  
    new ("FontSize",
    "E1C8"
  ),
  
    new ("Forward",
    "E111"
  ),
  
    new ("FourBars",
    "E1E9"
  ),
  
    new ("FullScreen",
    "E1D9"
  ),
  
    new ("FullScreenMaximize",
    "F8073"
  ),
  
    new ("FullScreenMinimize",
    "F8075"
  ),
  
    new ("Games",
    "F8077"
  ),
  
    new ("GamesFilled",
    "F8078"
  ),
  
    new ("GlobalNavigationButton",
    "E700"
  ),
  
    new ("Globe",
    "E12B"
  ),
  
    new ("GlobeFilled",
    "F8079"
  ),
  
    new ("Go",
    "E143"
  ),
  
    new ("GoToToday",
    "E184"
  ),
  
    new ("HangUp",
    "E137"
  ),
  
    new ("Help",
    "E11B"
  ),
  
    new ("Highlight",
    "E193"
  ),
  
    new ("HighlightFilled",
    "F807B"
  ),
  
    new ("Home",
    "E10F"
  ),
  
    new ("HomeFilled",
    "F807C"
  ),
  
    new ("Icons",
    "F807D"
  ),
  
    new ("IconsFilled",
    "F807E"
  ),
  
    new ("Image",
    "F807F"
  ),
  
    new ("ImageAltText",
    "F8080"
  ),
  
    new ("ImageAltTextFilled",
    "F8081"
  ),
  
    new ("ImageCopy",
    "F8082"
  ),
  
    new ("ImageCopyFilled",
    "F8083"
  ),
  
    new ("ImageEdit",
    "F8084"
  ),
  
    new ("ImageEditFilled",
    "F8085"
  ),
  
    new ("ImageFilled",
    "F8086"
  ),
  
    new ("Import",
    "E150"
  ),
  
    new ("ImportAll",
    "E151"
  ),
  
    new ("Important",
    "E171"
  ),
  
    new ("ImportantFilled",
    "F8087"
  ),
  
    new ("Italic",
    "E199"
  ),
  
    new ("Keyboard",
    "E144"
  ),
  
    new ("KeyboardFilled",
    "F808A"
  ),
  
    new ("LeaveChat",
    "E11F"
  ),
  
    new ("Library",
    "E1D3"
  ),
  
    new ("LibraryFilled",
    "F808B"
  ),
  
    new ("Like",
    "E19F"
  ),
  
    new ("LikeFilled",
    "F808C"
  ),
  
    new ("Link",
    "E167"
  ),
  
    new ("List",
    "E14C"
  ),
  
    new ("Mail",
    "E119"
  ),
  
    new ("MailFilled",
    "E135"
  ),
  
    new ("MailForward",
    "E120"
  ),
  
    new ("MailRead",
    "F808F"
  ),
  
    new ("MailReadAll",
    "F8090"
  ),
  
    new ("MailReadAllFilled",
    "F8091"
  ),
  
    new ("MailReadFilled",
    "F8092"
  ),
  
    new ("MailReply",
    "E172"
  ),
  
    new ("MailReplyAll",
    "E165"
  ),
  
    new ("MailReplyAllFilled",
    "F8093"
  ),
  
    new ("MailReplyFilled",
    "F8094"
  ),
  
    new ("MailUnread",
    "F8095"
  ),
  
    new ("MailUnreadAll",
    "F8096"
  ),
  
    new ("MailUnreadAllFilled",
    "F8097"
  ),
  
    new ("MailUnreadFilled",
    "F8098"
  ),
  
    new ("Manage",
    "E178"
  ),
  
    new ("Map",
    "E1C4"
  ),
  
    new ("MapDrive",
    "E17B"
  ),
  
    new ("MapDriveFilled",
    "F8099"
  ),
  
    new ("MapFilled",
    "F809A"
  ),
  
    new ("MapPin",
    "E139"
  ),
  
    new ("MapPinFilled",
    "F809B"
  ),
  
    new ("Memo",
    "E1D5"
  ),
  
    new ("Message",
    "E15F"
  ),
  
    new ("Microphone",
    "E1D6"
  ),
  
    new ("More",
    "E10C"
  ),
  
    new ("MoreVertical",
    "F809E"
  ),
  
    new ("MoveToFolder",
    "E19C"
  ),
  
    new ("Mute",
    "E198"
  ),
  
    new ("Navigation",
    "F80A0"
  ),
  
    new ("New",
    "F80A2"
  ),
  
    new ("NewFolder",
    "E1DA"
  ),
  
    new ("NewWindow",
    "E17C"
  ),
  
    new ("NewWindowFilled",
    "F80A4"
  ),
  
    new ("Next",
    "E101"
  ),
  
    new ("NextFilled",
    "F80A5"
  ),
  
    new ("OneBar",
    "E1E6"
  ),
  
    new ("Open",
    "F80A6"
  ),
  
    new ("OpenFile",
    "E1A5"
  ),
  
    new ("OpenFolder",
    "F80A8"
  ),
  
    new ("OpenFolderFilled",
    "F80A9"
  ),
  
    new ("OpenLocal",
    "E197"
  ),
  
    new ("Orientation",
    "E14F"
  ),
  
    new ("OrientationFilled",
    "F80AA"
  ),
  
    new ("OtherUser",
    "E1A6"
  ),
  
    new ("OutlineStar",
    "E1CE"
  ),
  
    new ("Page",
    "E132"
  ),
  
    new ("Page2",
    "E160"
  ),
  
    new ("PageFilled",
    "F80AB"
  ),
  
    new ("Paste",
    "E16D"
  ),
  
    new ("PasteFilled",
    "F80AC"
  ),
  
    new ("Pause",
    "E103"
  ),
  
    new ("PauseFilled",
    "F80AD"
  ),
  
    new ("People",
    "E125"
  ),
  
    new ("PeopleFilled",
    "F80AE"
  ),
  
    new ("Permissions",
    "E192"
  ),
  
    new ("Phone",
    "E13A"
  ),
  
    new ("PhoneBook",
    "E1D4"
  ),
  
    new ("PhoneFilled",
    "F80AF"
  ),
  
    new ("Pictures",
    "E158"
  ),
  
    new ("Pin",
    "E141"
  ),
  
    new ("Play",
    "E102"
  ),
  
    new ("PlayFilled",
    "F80B0"
  ),
  
    new ("Preview",
    "E295"
  ),
  
    new ("PreviewLink",
    "E12A"
  ),
  
    new ("PreviewLinkFilled",
    "F80B1"
  ),
  
    new ("Previous",
    "E100"
  ),
  
    new ("PreviousFilled",
    "F80B2"
  ),
  
    new ("Print",
    "E749"
  ),
  
    new ("PrintFilled",
    "F80B3"
  ),
  
    new ("ProtectedDocument",
    "E131"
  ),
  
    new ("RadioButton",
    "F80B4"
  ),
  
    new ("RadioButtonFilled",
    "F80B5"
  ),
  
    new ("Read",
    "E166"
  ),
  
    new ("Redo",
    "E10D"
  ),
  
    new ("Refresh",
    "E149"
  ),
  
    new ("Remote",
    "E148"
  ),
  
    new ("Remove",
    "E108"
  ),
  
    new ("Rename",
    "E13E"
  ),
  
    new ("Repair",
    "E15E"
  ),
  
    new ("RepeatAll",
    "E1CD"
  ),
  
    new ("RepeatOne",
    "E1CC"
  ),
  
    new ("ReportHacked",
    "E1DE"
  ),
  
    new ("Restore",
    "F80B9"
  ),
  
    new ("RestoreFilled",
    "F80BA"
  ),
  
    new ("Rotate",
    "E14A"
  ),
  
    new ("RotateCamera",
    "E124"
  ),
  
    new ("RotateClockwise",
    "F80BB"
  ),
  
    new ("RotateCounterClockwise",
    "F80BD"
  ),
  
    new ("Ruler",
    "ED5E"
  ),
  
    new ("RulerFilled",
    "F80C0"
  ),
  
    new ("Save",
    "E105"
  ),
  
    new ("SaveAs",
    "E792"
  ),
  
    new ("SaveAsFilled",
    "F80C2"
  ),
  
    new ("SaveFilled",
    "F80C3"
  ),
  
    new ("SaveLocal",
    "E159"
  ),
  
    new ("Scan",
    "E294"
  ),
  
    new ("SelectAll",
    "E14E"
  ),
  
    new ("SelectAllFilled",
    "F80C5"
  ),
  
    new ("Send",
    "E122"
  ),
  
    new ("SendFilled",
    "F80C6"
  ),
  
    new ("Setting",
    "E115"
  ),
  
    new ("Settings",
    "E713"
  ),
  
    new ("SettingsFilled",
    "F80C8"
  ),
  
    new ("Share",
    "E72D"
  ),
  
    new ("ShareAndroid",
    "F80C9"
  ),
  
    new ("ShareFilled",
    "F80CB"
  ),
  
    new ("ShareIOS",
    "F80CC"
  ),
  
    new ("ShareScreen",
    "F80CE"
  ),
  
    new ("ShareScreenFilled",
    "F80CF"
  ),
  
    new ("Shop",
    "E14D"
  ),
  
    new ("ShowResults",
    "E15C"
  ),
  
    new ("Shuffle",
    "E14B"
  ),
  
    new ("SlideShow",
    "E173"
  ),
  
    new ("SolidStar",
    "E1CF"
  ),
  
    new ("Sort",
    "E174"
  ),
  
    new ("Speaker0",
    "F80D1"
  ),
  
    new ("Speaker0Filled",
    "F80D2"
  ),
  
    new ("Speaker1",
    "F80D3"
  ),
  
    new ("Speaker1Filled",
    "F80D4"
  ),
  
    new ("Speaker2",
    "F80D5"
  ),
  
    new ("Speaker2Filled",
    "F80D6"
  ),
  
    new ("SpeakerBluetooth",
    "F80D7"
  ),
  
    new ("SpeakerBluetoothFilled",
    "F80D8"
  ),
  
    new ("SpeakerMute",
    "F80D9"
  ),
  
    new ("SpeakerMuteFilled",
    "F80DA"
  ),
  
    new ("SpeakerOff",
    "F80DB"
  ),
  
    new ("SpeakerOffFilled",
    "F80DC"
  ),
  
    new ("Star",
    "F80DD"
  ),
  
    new ("StarAdd",
    "F80DE"
  ),
  
    new ("StarAddFilled",
    "F80DF"
  ),
  
    new ("StarEmphasis",
    "F80E0"
  ),
  
    new ("StarEmphasisFilled",
    "F80E1"
  ),
  
    new ("StarFilled",
    "F80E2"
  ),
  
    new ("StarOff",
    "F80E3"
  ),
  
    new ("StarOffFilled",
    "F80E4"
  ),
  
    new ("StarProhibited",
    "F80E5"
  ),
  
    new ("StarProhibitedFilled",
    "F80E6"
  ),
  
    new ("Stop",
    "E15B"
  ),
  
    new ("StopFilled",
    "F80E7"
  ),
  
    new ("StopSlideShow",
    "E191"
  ),
  
    new ("Switch",
    "E13C"
  ),
  
    new ("Sync",
    "E117"
  ),
  
    new ("SyncFolder",
    "E1DF"
  ),
  
    new ("Tag",
    "E1CB"
  ),
  
    new ("TagFilled",
    "F80E9"
  ),
  
    new ("Target",
    "E1D2"
  ),
  
    new ("TargetEdit",
    "F80EA"
  ),
  
    new ("ThreeBars",
    "E1E8"
  ),
  
    new ("TwoBars",
    "E1E7"
  ),
  
    new ("TwoPage",
    "E11E"
  ),
  
    new ("Underline",
    "E19A"
  ),
  
    new ("Undo",
    "E10E"
  ),
  
    new ("UnFavorite",
    "E195"
  ),
  
    new ("UnPin",
    "E196"
  ),
  
    new ("Up",
    "E110"
  ),
  
    new ("Upload",
    "E11C"
  ),
  
    new ("Video",
    "E116"
  ),
  
    new ("VideoFilled",
    "F80F1"
  ),
  
    new ("View",
    "E18B"
  ),
  
    new ("ViewAll",
    "E138"
  ),
  
    new ("Volume",
    "E15D"
  ),
  
    new ("WeatherBlowingSnow",
    "F80F2"
  ),
  
    new ("WeatherCloudy",
    "F80F4"
  ),
  
    new ("WeatherCloudyFilled",
    "F80F5"
  ),
  
    new ("WeatherDrizzle",
    "F80F6"
  ),
  
    new ("WeatherDrizzleFilled",
    "F80F7"
  ),
  
    new ("WeatherDustStorm",
    "F80F8"
  ),
  
    new ("WeatherFog",
    "F80FA"
  ),
  
    new ("WeatherFogFilled",
    "F80FB"
  ),
  
    new ("WeatherHailDay",
    "F80FC"
  ),
  
    new ("WeatherHailDayFilled",
    "F80FD"
  ),
  
    new ("WeatherHailNight",
    "F80FE"
  ),
  
    new ("WeatherHailNightFilled",
    "F80FF"
  ),
  
    new ("WeatherHaze",
    "F8100"
  ),
  
    new ("WeatherHazeFilled",
    "F8101"
  ),
  
    new ("WeatherMoon",
    "F8102"
  ),
  
    new ("WeatherMoonFilled",
    "F8103"
  ),
  
    new ("WeatherPartlyCloudyDay",
    "F8104"
  ),
  
    new ("WeatherPartlyCloudyDayFilled",
    "F8105"
  ),
  
    new ("WeatherPartlyCloudyNight",
    "F8106"
  ),
  
    new ("WeatherPartlyCloudyNightFilled",
    "F8107"
  ),
  
    new ("WeatherRain",
    "F8108"
  ),
  
    new ("WeatherRainFilled",
    "F8109"
  ),
  
    new ("WeatherRainShowersDay",
    "F810A"
  ),
  
    new ("WeatherRainShowersDayFilled",
    "F810B"
  ),
  
    new ("WeatherRainShowersNight",
    "F810C"
  ),
  
    new ("WeatherRainShowersNightFilled",
    "F810D"
  ),
  
    new ("WeatherRainSnow",
    "F810E"
  ),
  
    new ("WeatherRainSnowFilled",
    "F810F"
  ),
  
    new ("WeatherSnow",
    "F8110"
  ),
  
    new ("WeatherSnowFilled",
    "F8111"
  ),
  
    new ("WeatherSnowflake",
    "F8112"
  ),
  
    new ("WeatherSnowShowerDay",
    "F8114"
  ),
  
    new ("WeatherSnowShowerDayFilled",
    "F8115"
  ),
  
    new ("WeatherSnowShowerNight",
    "F8116"
  ),
  
    new ("WeatherSnowShowerNightFilled",
    "F8117"
  ),
  
    new ("WeatherSqualls",
    "F8118"
  ),
  
    new ("WeatherSunny",
    "F811A"
  ),
  
    new ("WeatherSunnyFilled",
    "F811B"
  ),
  
    new ("WeatherSunnyHigh",
    "F811C"
  ),
  
    new ("WeatherSunnyHighFilled",
    "F811D"
  ),
  
    new ("WeatherSunnyLow",
    "F811E"
  ),
  
    new ("WeatherSunnyLowFilled",
    "F811F"
  ),
  
    new ("WeatherThunderstorm",
    "F8120"
  ),
  
    new ("WeatherThunderstormFilled",
    "F8121"
  ),
  
    new ("WebCam",
    "E156"
  ),
  
    new ("Wifi1",
    "E872"
  ),
  
    new ("Wifi2",
    "E873"
  ),
  
    new ("Wifi3",
    "E874"
  ),
  
    new ("Wifi4",
    "E701"
  ),
  
    new ("WifiProtected",
    "F812A"
  ),
  
    new ("WifiProtectedFilled",
    "F812B"
  ),
  
    new ("WifiWarning",
    "F812C"
  ),
  
    new ("WifiWarningFilled",
    "F812D"
  ),
  
    new ("World",
    "E128"
  ),
  
    new ("XboxConsole",
    "F812E"
  ),
  
    new ("XboxConsoleFilled",
    "F812F"
  ),
  
    new ("XboxOneConsole",
    "E990"
  ),
  
    new ("ZeroBars",
    "E1E5"
  ),
  
    new ("ZipFolder",
    "F012"
  ),
  
    new ("ZipFolderFilled",
    "F8131"
  ),
  
    new ("Zoom",
    "E1A3"
  ),
  
    new ("ZoomIn",
    "E12E"
  ),
  
    new ("ZoomInFilled",
    "F8132"
  ),
  
    new ("ZoomOut",
    "E1A4"
  ),
  
    new ("ZoomOutFilled",
    "F8133"
  ),
  
    new ("GlobalNavButton",
    "E700"
  ),
  
    new ("Wifi",
    "E701"
  ),
  
    new ("Bluetooth",
    "E702"
  ),
  
    new ("Connect",
    "E703"
  ),
  
    new ("VPN",
    "E705"
  ),
  
    new ("Brightness",
    "E706"
  ),
  
    new ("MapPin",
    "E707"
  ),
  
    new ("QuietHours",
    "E708"
  ),
  
    new ("Airplane",
    "E709"
  ),
  
    new ("Tablet",
    "E70A"
  ),
  
    new ("QuickNote",
    "E70B"
  ),
  
    new ("ChevronDown",
    "E70D"
  ),
  
    new ("ChevronUp",
    "E70E"
  ),
  
    new ("Edit",
    "E70F"
  ),
  
    new ("Add",
    "E710"
  ),
  
    new ("Cancel",
    "E711"
  ),
  
    new ("More",
    "E712"
  ),
  
    new ("Settings",
    "E713"
  ),
  
    new ("Video",
    "E714"
  ),
  
    new ("Mail",
    "E715"
  ),
  
    new ("People",
    "E716"
  ),
  
    new ("Phone",
    "E717"
  ),
  
    new ("Pin",
    "E718"
  ),
  
    new ("Shop",
    "E719"
  ),
  
    new ("Stop",
    "E71A"
  ),
  
    new ("Link",
    "E71B"
  ),
  
    new ("Filter",
    "E71C"
  ),
  
    new ("AllApps",
    "E71D"
  ),
  
    new ("Zoom",
    "E71E"
  ),
  
    new ("ZoomOut",
    "E71F"
  ),
  
    new ("Microphone",
    "E720"
  ),
  
    new ("Search",
    "E721"
  ),
  
    new ("Camera",
    "E722"
  ),
  
    new ("Attach",
    "E723"
  ),
  
    new ("Send",
    "E724"
  ),
  
    new ("SendFill",
    "E725"
  ),
  
    new ("WalkSolid",
    "E726"
  ),
  
    new ("InPrivate",
    "E727"
  ),
  
    new ("FavoriteList",
    "E728"
  ),
  
    new ("PageSolid",
    "E729"
  ),
  
    new ("Forward",
    "E72A"
  ),
  
    new ("Back",
    "E72B"
  ),
  
    new ("Refresh",
    "E72C"
  ),
  
    new ("Share",
    "E72D"
  ),
  
    new ("Lock",
    "E72E"
  ),
  
    new ("ReportHacked",
    "E730"
  ),
  
    new ("EMI",
    "E731"
  ),
  
    new ("FavoriteStar",
    "E734"
  ),
  
    new ("FavoriteStarFill",
    "E735"
  ),
  
    new ("ReadingMode",
    "E736"
  ),
  
    new ("Favicon",
    "E737"
  ),
  
    new ("Remove",
    "E738"
  ),
  
    new ("Checkbox",
    "E739"
  ),
  
    new ("CheckboxComposite",
    "E73A"
  ),
  
    new ("CheckboxFill",
    "E73B"
  ),
  
    new ("CheckboxIndeterminate",
    "E73C"
  ),
  
    new ("CheckboxCompositeReversed",
    "E73D"
  ),
  
    new ("CheckMark",
    "E73E"
  ),
  
    new ("BackToWindow",
    "E73F"
  ),
  
    new ("FullScreen",
    "E740"
  ),
  
    new ("ResizeTouchLarger",
    "E741"
  ),
  
    new ("ResizeTouchSmaller",
    "E742"
  ),
  
    new ("ResizeMouseSmall",
    "E743"
  ),
  
    new ("ResizeMouseMedium",
    "E744"
  ),
  
    new ("SwitchUser",
    "E748"
  ),
  
    new ("Print",
    "E749"
  ),
  
    new ("Up",
    "E74A"
  ),
  
    new ("Down",
    "E74B"
  ),
  
    new ("OEM",
    "E74C"
  ),
  
    new ("Delete",
    "E74D"
  ),
  
    new ("Save",
    "E74E"
  ),
  
    new ("Mute",
    "E74F"
  ),
  
    new ("BackSpaceQWERTY",
    "E750"
  ),
  
    new ("ReturnKey",
    "E751"
  ),
  
    new ("UpArrowShiftKey",
    "E752"
  ),
  
    new ("Cloud",
    "E753"
  ),
  
    new ("Flashlight",
    "E754"
  ),
  
    new ("EraseTool",
    "E75C"
  ),
  
    new ("UnderscoreSpace",
    "E75D"
  ),
  
    new ("Dialpad",
    "E75F"
  ),
  
    new ("PageLeft",
    "E760"
  ),
  
    new ("PageRight",
    "E761"
  ),
  
    new ("MultiSelect",
    "E762"
  ),
  
    new ("KeyboardLeftHanded",
    "E763"
  ),
  
    new ("KeyboardClassic",
    "E765"
  ),
  
    new ("KeyboardSplit",
    "E766"
  ),
  
    new ("Volume",
    "E767"
  ),
  
    new ("Play",
    "E768"
  ),
  
    new ("Pause",
    "E769"
  ),
  
    new ("ChevronLeft",
    "E76B"
  ),
  
    new ("ChevronRight",
    "E76C"
  ),
  
    new ("InkingTool",
    "E76D"
  ),
  
    new ("Emoji2",
    "E76E"
  ),
  
    new ("GripperBarHorizontal",
    "E76F"
  ),
  
    new ("System",
    "E770"
  ),
  
    new ("Personalize",
    "E771"
  ),
  
    new ("SearchAndApps",
    "E773"
  ),
  
    new ("Globe",
    "E774"
  ),
  
    new ("EaseOfAccess",
    "E776"
  ),
  
    new ("UpdateRestore",
    "E777"
  ),
  
    new ("HangUp",
    "E778"
  ),
  
    new ("ContactInfo",
    "E779"
  ),
  
    new ("Unpin",
    "E77A"
  ),
  
    new ("Contact",
    "E77B"
  ),
  
    new ("Memo",
    "E77C"
  ),
  
    new ("IncomingCall",
    "E77E"
  ),
  
    new ("Paste",
    "E77F"
  ),
  
    new ("PhoneBook",
    "E780"
  ),
  
    new ("Error",
    "E783"
  ),
  
    new ("GripperBarVertical",
    "E784"
  ),
  
    new ("Unlock",
    "E785"
  ),
  
    new ("Slideshow",
    "E786"
  ),
  
    new ("Calendar",
    "E787"
  ),
  
    new ("GripperResize",
    "E788"
  ),
  
    new ("Megaphone",
    "E789"
  ),
  
    new ("NewWindow",
    "E78B"
  ),
  
    new ("SaveLocal",
    "E78C"
  ),
  
    new ("Color",
    "E790"
  ),
  
    new ("SaveAs",
    "E792"
  ),
  
    new ("AspectRatio",
    "E799"
  ),
  
    new ("Redo",
    "E7A6"
  ),
  
    new ("Undo",
    "E7A7"
  ),
  
    new ("Crop",
    "E7A8"
  ),
  
    new ("Rotate",
    "E7AD"
  ),
  
    new ("RedEye",
    "E7B3"
  ),
  
    new ("MapPin2",
    "E7B7"
  ),
  
    new ("Warning",
    "E7BA"
  ),
  
    new ("ReadingList",
    "E7BC"
  ),
  
    new ("Education",
    "E7BE"
  ),
  
    new ("ShoppingCart",
    "E7BF"
  ),
  
    new ("Train",
    "E7C0"
  ),
  
    new ("Flag",
    "E7C1"
  ),
  
    new ("Move",
    "E7C2"
  ),
  
    new ("Page",
    "E7C3"
  ),
  
    new ("BrowsePhotos",
    "E7C5"
  ),
  
    new ("HalfStarLeft",
    "E7C6"
  ),
  
    new ("Record",
    "E7C8"
  ),
  
    new ("Ferry",
    "E7E3"
  ),
  
    new ("Highlight",
    "E7E6"
  ),
  
    new ("ActionCenterNotification",
    "E7E7"
  ),
  
    new ("PowerButton",
    "E7E8"
  ),
  
    new ("ResizeTouchNarrower",
    "E7EA"
  ),
  
    new ("ResizeTouchShorter",
    "E7EB"
  ),
  
    new ("DrivingMode",
    "E7EC"
  ),
  
    new ("RingerSilent",
    "E7ED"
  ),
  
    new ("OtherUser",
    "E7EE"
  ),
  
    new ("Admin",
    "E7EF"
  ),
  
    new ("CC",
    "E7F0"
  ),
  
    new ("SDCard",
    "E7F1"
  ),
  
    new ("CallForwarding",
    "E7F2"
  ),
  
    new ("TVMonitor",
    "E7F4"
  ),
  
    new ("Headphone",
    "E7F6"
  ),
  
    new ("DeviceLaptopPic",
    "E7F7"
  ),
  
    new ("DeviceLaptopNoPic",
    "E7F8"
  ),
  
    new ("DeviceMonitorRightPic",
    "E7F9"
  ),
  
    new ("DeviceMonitorLeftPic",
    "E7FA"
  ),
  
    new ("DeviceMonitorNoPic",
    "E7FB"
  ),
  
    new ("Game",
    "E7FC"
  ),
  
    new ("HorizontalTabKey",
    "E7FD"
  ),
  
    new ("Car",
    "E804"
  ),
  
    new ("Walk",
    "E805"
  ),
  
    new ("Bus",
    "E806"
  ),
  
    new ("TiltDown",
    "E80A"
  ),
  
    new ("RotateMapRight",
    "E80C"
  ),
  
    new ("RotateMapLeft",
    "E80D"
  ),
  
    new ("Home",
    "E80F"
  ),
  
    new ("ParkingLocation",
    "E811"
  ),
  
    new ("IncidentTriangle",
    "E814"
  ),
  
    new ("Touch",
    "E815"
  ),
  
    new ("MapDirections",
    "E816"
  ),
  
    new ("EndPoint",
    "E81B"
  ),
  
    new ("History",
    "E81C"
  ),
  
    new ("Location",
    "E81D"
  ),
  
    new ("MapLayers",
    "E81E"
  ),
  
    new ("Accident",
    "E81F"
  ),
  
    new ("Work",
    "E821"
  ),
  
    new ("Construction",
    "E822"
  ),
  
    new ("Recent",
    "E823"
  ),
  
    new ("Bank",
    "E825"
  ),
  
    new ("DownloadMap",
    "E826"
  ),
  
    new ("InkingToolFill2",
    "E829"
  ),
  
    new ("EraseToolFill",
    "E82B"
  ),
  
    new ("EraseToolFill2",
    "E82C"
  ),
  
    new ("Dictionary",
    "E82D"
  ),
  
    new ("DictionaryAdd",
    "E82E"
  ),
  
    new ("ToolTip",
    "E82F"
  ),
  
    new ("ChromeBack",
    "E830"
  ),
  
    new ("FolderOpen",
    "E838"
  ),
  
    new ("DirectAccess",
    "E83B"
  ),
  
    new ("DefenderApp",
    "E83D"
  ),
  
    new ("BatteryCharging9",
    "E83E"
  ),
  
    new ("Battery10",
    "E83F"
  ),
  
    new ("Pinned",
    "E840"
  ),
  
    new ("PinFill",
    "E841"
  ),
  
    new ("PinnedFill",
    "E842"
  ),
  
    new ("RevToggleKey",
    "E845"
  ),
  
    new ("Battery0",
    "E850"
  ),
  
    new ("Battery1",
    "E851"
  ),
  
    new ("Battery2",
    "E852"
  ),
  
    new ("Battery3",
    "E853"
  ),
  
    new ("Battery4",
    "E854"
  ),
  
    new ("Battery5",
    "E855"
  ),
  
    new ("Battery6",
    "E856"
  ),
  
    new ("Battery7",
    "E857"
  ),
  
    new ("Battery8",
    "E858"
  ),
  
    new ("Battery9",
    "E859"
  ),
  
    new ("BatteryCharging0",
    "E85A"
  ),
  
    new ("BatteryCharging1",
    "E85B"
  ),
  
    new ("BatteryCharging2",
    "E85C"
  ),
  
    new ("BatteryCharging3",
    "E85D"
  ),
  
    new ("BatteryCharging4",
    "E85E"
  ),
  
    new ("BatteryCharging5",
    "E85F"
  ),
  
    new ("BatteryCharging6",
    "E860"
  ),
  
    new ("BatteryCharging7",
    "E861"
  ),
  
    new ("BatteryCharging8",
    "E862"
  ),
  
    new ("BatterySaver0",
    "E863"
  ),
  
    new ("BatterySaver1",
    "E864"
  ),
  
    new ("BatterySaver2",
    "E865"
  ),
  
    new ("BatterySaver3",
    "E866"
  ),
  
    new ("BatterySaver4",
    "E867"
  ),
  
    new ("BatterySaver5",
    "E868"
  ),
  
    new ("BatterySaver6",
    "E869"
  ),
  
    new ("BatterySaver7",
    "E86A"
  ),
  
    new ("BatterySaver8",
    "E86B"
  ),
  
    new ("SignalBars1",
    "E86C"
  ),
  
    new ("SignalBars2",
    "E86D"
  ),
  
    new ("SignalBars3",
    "E86E"
  ),
  
    new ("SignalBars4",
    "E86F"
  ),
  
    new ("SignalBars5",
    "E870"
  ),
  
    new ("SignalNotConnected",
    "E871"
  ),
  
    new ("Wifi1",
    "E872"
  ),
  
    new ("Wifi2",
    "E873"
  ),
  
    new ("Wifi3",
    "E874"
  ),
  
    new ("RoamingInternational",
    "E878"
  ),
  
    new ("RoamingDomestic",
    "E879"
  ),
  
    new ("JpnRomanjiShiftLock",
    "E87F"
  ),
  
    new ("USB",
    "E88E"
  ),
  
    new ("InkingToolFill",
    "E88F"
  ),
  
    new ("View",
    "E890"
  ),
  
    new ("HighlightFill",
    "E891"
  ),
  
    new ("Previous",
    "E892"
  ),
  
    new ("Next",
    "E893"
  ),
  
    new ("Clear",
    "E894"
  ),
  
    new ("Sync",
    "E895"
  ),
  
    new ("Download",
    "E896"
  ),
  
    new ("Help",
    "E897"
  ),
  
    new ("Upload",
    "E898"
  ),
  
    new ("Emoji",
    "E899"
  ),
  
    new ("TwoPage",
    "E89A"
  ),
  
    new ("LeaveChat",
    "E89B"
  ),
  
    new ("MailForward",
    "E89C"
  ),
  
    new ("RotateCamera",
    "E89E"
  ),
  
    new ("PreviewLink",
    "E8A1"
  ),
  
    new ("AttachCamera",
    "E8A2"
  ),
  
    new ("ZoomIn",
    "E8A3"
  ),
  
    new ("Bookmarks",
    "E8A4"
  ),
  
    new ("Document",
    "E8A5"
  ),
  
    new ("ProtectedDocument",
    "E8A6"
  ),
  
    new ("OpenInNewWindow",
    "E8A7"
  ),
  
    new ("MailFill",
    "E8A8"
  ),
  
    new ("ViewAll",
    "E8A9"
  ),
  
    new ("Switch",
    "E8AB"
  ),
  
    new ("Rename",
    "E8AC"
  ),
  
    new ("Go",
    "E8AD"
  ),
  
    new ("Remote",
    "E8AF"
  ),
  
    new ("Click",
    "E8B0"
  ),
  
    new ("Shuffle",
    "E8B1"
  ),
  
    new ("Movies",
    "E8B2"
  ),
  
    new ("SelectAll",
    "E8B3"
  ),
  
    new ("Orientation",
    "E8B4"
  ),
  
    new ("Import",
    "E8B5"
  ),
  
    new ("ImportAll",
    "E8B6"
  ),
  
    new ("Folder",
    "E8B7"
  ),
  
    new ("Webcam",
    "E8B8"
  ),
  
    new ("Picture",
    "E8B9"
  ),
  
    new ("ChromeClose",
    "E8BB"
  ),
  
    new ("ShowResults",
    "E8BC"
  ),
  
    new ("Message",
    "E8BD"
  ),
  
    new ("Leaf",
    "E8BE"
  ),
  
    new ("CalendarDay",
    "E8BF"
  ),
  
    new ("CalendarWeek",
    "E8C0"
  ),
  
    new ("Characters",
    "E8C1"
  ),
  
    new ("MailReplyAll",
    "E8C2"
  ),
  
    new ("Read",
    "E8C3"
  ),
  
    new ("Cut",
    "E8C6"
  ),
  
    new ("Copy",
    "E8C8"
  ),
  
    new ("Important",
    "E8C9"
  ),
  
    new ("MailReply",
    "E8CA"
  ),
  
    new ("Sort",
    "E8CB"
  ),
  
    new ("MobileTablet",
    "E8CC"
  ),
  
    new ("MapDrive",
    "E8CE"
  ),
  
    new ("GotoToday",
    "E8D1"
  ),
  
    new ("Font",
    "E8D2"
  ),
  
    new ("FontColor",
    "E8D3"
  ),
  
    new ("Contact2",
    "E8D4"
  ),
  
    new ("FolderFill",
    "E8D5"
  ),
  
    new ("Audio",
    "E8D6"
  ),
  
    new ("Permissions",
    "E8D7"
  ),
  
    new ("Unfavorite",
    "E8D9"
  ),
  
    new ("OpenLocal",
    "E8DA"
  ),
  
    new ("Italic",
    "E8DB"
  ),
  
    new ("Underline",
    "E8DC"
  ),
  
    new ("Bold",
    "E8DD"
  ),
  
    new ("MoveToFolder",
    "E8DE"
  ),
  
    new ("Dislike",
    "E8E0"
  ),
  
    new ("Like",
    "E8E1"
  ),
  
    new ("AlignRight",
    "E8E2"
  ),
  
    new ("AlignCenter",
    "E8E3"
  ),
  
    new ("AlignLeft",
    "E8E4"
  ),
  
    new ("OpenFile",
    "E8E5"
  ),
  
    new ("ClearSelection",
    "E8E6"
  ),
  
    new ("FontDecrease",
    "E8E7"
  ),
  
    new ("FontIncrease",
    "E8E8"
  ),
  
    new ("FontSize",
    "E8E9"
  ),
  
    new ("CellPhone",
    "E8EA"
  ),
  
    new ("Tag",
    "E8EC"
  ),
  
    new ("RepeatOne",
    "E8ED"
  ),
  
    new ("RepeatAll",
    "E8EE"
  ),
  
    new ("Calculator",
    "E8EF"
  ),
  
    new ("Directions",
    "E8F0"
  ),
  
    new ("Library",
    "E8F1"
  ),
  
    new ("ChatBubbles",
    "E8F2"
  ),
  
    new ("NewFolder",
    "E8F4"
  ),
  
    new ("CalendarReply",
    "E8F5"
  ),
  
    new ("SyncFolder",
    "E8F7"
  ),
  
    new ("BlockContact",
    "E8F8"
  ),
  
    new ("AddFriend",
    "E8FA"
  ),
  
    new ("Accept",
    "E8FB"
  ),
  
    new ("BulletedList",
    "E8FD"
  ),
  
    new ("Scan",
    "E8FE"
  ),
  
    new ("Preview",
    "E8FF"
  ),
  
    new ("Group",
    "E902"
  ),
  
    new ("ZeroBars",
    "E904"
  ),
  
    new ("OneBar",
    "E905"
  ),
  
    new ("TwoBars",
    "E906"
  ),
  
    new ("ThreeBars",
    "E907"
  ),
  
    new ("FourBars",
    "E908"
  ),
  
    new ("World",
    "E909"
  ),
  
    new ("Comment",
    "E90A"
  ),
  
    new ("DockLeft",
    "E90C"
  ),
  
    new ("DockRight",
    "E90D"
  ),
  
    new ("DockBottom",
    "E90E"
  ),
  
    new ("Repair",
    "E90F"
  ),
  
    new ("Accounts",
    "E910"
  ),
  
    new ("Manage",
    "E912"
  ),
  
    new ("RadioBullet",
    "E915"
  ),
  
    new ("Stopwatch",
    "E916"
  ),
  
    new ("ActionCenter",
    "E91C"
  ),
  
    new ("FullCircleMask",
    "E91F"
  ),
  
    new ("ChromeMinimize",
    "E921"
  ),
  
    new ("ChromeMaximize",
    "E922"
  ),
  
    new ("ChromeRestore",
    "E923"
  ),
  
    new ("Annotation",
    "E924"
  ),
  
    new ("BackSpaceQWERTYSm",
    "E925"
  ),
  
    new ("BackSpaceQWERTYMd",
    "E926"
  ),
  
    new ("Swipe",
    "E927"
  ),
  
    new ("Fingerprint",
    "E928"
  ),
  
    new ("ChromeBackToWindow",
    "E92C"
  ),
  
    new ("ChromeFullScreen",
    "E92D"
  ),
  
    new ("KeyboardStandard",
    "E92E"
  ),
  
    new ("KeyboardDismiss",
    "E92F"
  ),
  
    new ("Completed",
    "E930"
  ),
  
    new ("ChromeAnnotate",
    "E931"
  ),
  
    new ("IBeam",
    "E933"
  ),
  
    new ("IBeamOutline",
    "E934"
  ),
  
    new ("FeedbackApp",
    "E939"
  ),
  
    new ("Code",
    "E943"
  ),
  
    new ("LightningBolt",
    "E945"
  ),
  
    new ("Info",
    "E946"
  ),
  
    new ("CalculatorMultiply",
    "E947"
  ),
  
    new ("CalculatorAddition",
    "E948"
  ),
  
    new ("CalculatorSubtract",
    "E949"
  ),
  
    new ("CalculatorDivide",
    "E94A"
  ),
  
    new ("CalculatorSquareroot",
    "E94B"
  ),
  
    new ("CalculatorPercentage",
    "E94C"
  ),
  
    new ("CalculatorEqualTo",
    "E94E"
  ),
  
    new ("CalculatorBackspace",
    "E94F"
  ),
  
    new ("StorageOptical",
    "E958"
  ),
  
    new ("Headset",
    "E95B"
  ),
  
    new ("Health",
    "E95E"
  ),
  
    new ("Webcam2",
    "E960"
  ),
  
    new ("Input",
    "E961"
  ),
  
    new ("Mouse",
    "E962"
  ),
  
    new ("ReturnKeySm",
    "E966"
  ),
  
    new ("GameConsole",
    "E967"
  ),
  
    new ("ChevronUpSmall",
    "E96D"
  ),
  
    new ("ChevronDownSmall",
    "E96E"
  ),
  
    new ("ChevronLeftSmall",
    "E96F"
  ),
  
    new ("ChevronRightSmall",
    "E970"
  ),
  
    new ("ChevronUpMed",
    "E971"
  ),
  
    new ("ChevronDownMed",
    "E972"
  ),
  
    new ("ChevronLeftMed",
    "E973"
  ),
  
    new ("ChevronRightMed",
    "E974"
  ),
  
    new ("Devices2",
    "E975"
  ),
  
    new ("PresenceChicklet",
    "E978"
  ),
  
    new ("PresenceChickletVideo",
    "E979"
  ),
  
    new ("Reply",
    "E97A"
  ),
  
    new ("ConstructionCone",
    "E98F"
  ),
  
    new ("XboxOneConsole",
    "E990"
  ),
  
    new ("Volume0",
    "E992"
  ),
  
    new ("Volume1",
    "E993"
  ),
  
    new ("Volume2",
    "E994"
  ),
  
    new ("Volume3",
    "E995"
  ),
  
    new ("Robot",
    "E99A"
  ),
  
    new ("FitPage",
    "E9A6"
  ),
  
    new ("ForwardSm",
    "E9AC"
  ),
  
    new ("Frigid",
    "E9CA"
  ),
  
    new ("Unknown",
    "E9CE"
  ),
  
    new ("AreaChart",
    "E9D2"
  ),
  
    new ("CheckList",
    "E9D5"
  ),
  
    new ("Diagnostic",
    "E9D9"
  ),
  
    new ("Equalizer",
    "E9E9"
  ),
  
    new ("Processing",
    "E9F5"
  ),
  
    new ("ReportDocument",
    "E9F9"
  ),
  
    new ("VideoSolid",
    "EA0C"
  ),
  
    new ("Shield",
    "EA18"
  ),
  
    new ("Info2",
    "EA1F"
  ),
  
    new ("SaveCopy",
    "EA35"
  ),
  
    new ("List",
    "EA37"
  ),
  
    new ("Asterisk",
    "EA38"
  ),
  
    new ("ErrorBadge",
    "EA39"
  ),
  
    new ("CircleRing",
    "EA3A"
  ),
  
    new ("CircleFill",
    "EA3B"
  ),
  
    new ("Record2",
    "EA3F"
  ),
  
    new ("AllAppsMirrored",
    "EA40"
  ),
  
    new ("BookmarksMirrored",
    "EA41"
  ),
  
    new ("BulletedListMirrored",
    "EA42"
  ),
  
    new ("ChromeBackMirrored",
    "EA47"
  ),
  
    new ("DockRightMirrored",
    "EA4B"
  ),
  
    new ("DockLeftMirrored",
    "EA4C"
  ),
  
    new ("ExpandTileMirrored",
    "EA4E"
  ),
  
    new ("GoMirrored",
    "EA4F"
  ),
  
    new ("ListMirrored",
    "EA55"
  ),
  
    new ("ParkingLocationMirrored",
    "EA5E"
  ),
  
    new ("ResizeTouchNarrowerMirrored",
    "EA62"
  ),
  
    new ("SendMirrored",
    "EA63"
  ),
  
    new ("SendFillMirrored",
    "EA64"
  ),
  
    new ("Devices3",
    "EA6C"
  ),
  
    new ("SlowMotionOn",
    "EA79"
  ),
  
    new ("Lightbulb",
    "EA80"
  ),
  
    new ("Puzzle",
    "EA86"
  ),
  
    new ("CalendarSolid",
    "EA89"
  ),
  
    new ("HomeSolid",
    "EA8A"
  ),
  
    new ("ParkingLocationSolid",
    "EA8B"
  ),
  
    new ("ContactSolid",
    "EA8C"
  ),
  
    new ("ConstructionSolid",
    "EA8D"
  ),
  
    new ("AccidentSolid",
    "EA8E"
  ),
  
    new ("Ringer",
    "EA8F"
  ),
  
    new ("PDF",
    "EA90"
  ),
  
    new ("HeartBroken",
    "EA92"
  ),
  
    new ("BatterySaver9",
    "EA94"
  ),
  
    new ("BatterySaver10",
    "EA95"
  ),
  
    new ("Broom",
    "EA99"
  ),
  
    new ("ForwardCall",
    "EAC2"
  ),
  
    new ("Market",
    "EAFC"
  ),
  
    new ("PieSingle",
    "EB05"
  ),
  
    new ("StockDown",
    "EB0F"
  ),
  
    new ("StockUp",
    "EB11"
  ),
  
    new ("Drop",
    "EB42"
  ),
  
    new ("Radar",
    "EB44"
  ),
  
    new ("BusSolid",
    "EB47"
  ),
  
    new ("FerrySolid",
    "EB48"
  ),
  
    new ("EndPointSolid",
    "EB4B"
  ),
  
    new ("AirplaneSolid",
    "EB4C"
  ),
  
    new ("TrainSolid",
    "EB4D"
  ),
  
    new ("WorkSolid",
    "EB4E"
  ),
  
    new ("ReminderFill",
    "EB4F"
  ),
  
    new ("Reminder",
    "EB50"
  ),
  
    new ("Heart",
    "EB51"
  ),
  
    new ("HeartFill",
    "EB52"
  ),
  
    new ("WifiWarning4",
    "EB63"
  ),
  
    new ("EditMirrored",
    "EB7E"
  ),
  
    new ("StatusErrorFull",
    "EB90"
  ),
  
    new ("BackSpaceQWERTYLg",
    "EB96"
  ),
  
    new ("ReturnKeyLg",
    "EB97"
  ),
  
    new ("FastForward",
    "EB9D"
  ),
  
    new ("Rewind",
    "EB9E"
  ),
  
    new ("MobBattery0",
    "EBA0"
  ),
  
    new ("MobBattery1",
    "EBA1"
  ),
  
    new ("MobBattery2",
    "EBA2"
  ),
  
    new ("MobBattery3",
    "EBA3"
  ),
  
    new ("MobBattery4",
    "EBA4"
  ),
  
    new ("MobBattery5",
    "EBA5"
  ),
  
    new ("MobBattery6",
    "EBA6"
  ),
  
    new ("MobBattery7",
    "EBA7"
  ),
  
    new ("MobBattery8",
    "EBA8"
  ),
  
    new ("MobBattery9",
    "EBA9"
  ),
  
    new ("MobBattery10",
    "EBAA"
  ),
  
    new ("CloudDownload",
    "EBD3"
  ),
  
    new ("Family",
    "EBDA"
  ),
  
    new ("RightArrowKeyTime0",
    "EBE7"
  ),
  
    new ("Bug",
    "EBE8"
  ),
  
    new ("CityNext",
    "EC06"
  ),
  
    new ("CityNext2",
    "EC07"
  ),
  
    new ("Courthouse",
    "EC08"
  ),
  
    new ("Sustainable",
    "EC0A"
  ),
  
    new ("MiracastLogoSmall",
    "EC15"
  ),
  
    new ("MiracastLogoLarge",
    "EC16"
  ),
  
    new ("PersonalFolder",
    "EC25"
  ),
  
    new ("KeyboardFull",
    "EC31"
  ),
  
    new ("Cafe",
    "EC32"
  ),
  
    new ("MobSignal1",
    "EC37"
  ),
  
    new ("MobSignal2",
    "EC38"
  ),
  
    new ("MobSignal3",
    "EC39"
  ),
  
    new ("MobSignal4",
    "EC3A"
  ),
  
    new ("MobSignal5",
    "EC3B"
  ),
  
    new ("MobWifi1",
    "EC3C"
  ),
  
    new ("MobWifi2",
    "EC3D"
  ),
  
    new ("MobWifi3",
    "EC3E"
  ),
  
    new ("MobWifi4",
    "EC3F"
  ),
  
    new ("MobAirplane",
    "EC40"
  ),
  
    new ("MobBluetooth",
    "EC41"
  ),
  
    new ("MobLocation",
    "EC43"
  ),
  
    new ("MobQuietHours",
    "EC46"
  ),
  
    new ("MobDrivingMode",
    "EC47"
  ),
  
    new ("SpeedHigh",
    "EC4A"
  ),
  
    new ("MusicNote",
    "EC4F"
  ),
  
    new ("FileExplorer",
    "EC50"
  ),
  
    new ("FileExplorerApp",
    "EC51"
  ),
  
    new ("LeftArrowKeyTime0",
    "EC52"
  ),
  
    new ("MicOff",
    "EC54"
  ),
  
    new ("PlaybackRate1x",
    "EC57"
  ),
  
    new ("PlaybackRateOther",
    "EC58"
  ),
  
    new ("CashDrawer",
    "EC59"
  ),
  
    new ("CompletedSolid",
    "EC61"
  ),
  
    new ("MicOn",
    "EC71"
  ),
  
    new ("DeveloperTools",
    "EC7A"
  ),
  
    new ("MobCallForwarding",
    "EC7E"
  ),
  
    new ("ScrollUpDown",
    "EC8F"
  ),
  
    new ("DateTime",
    "EC92"
  ),
  
    new ("Tiles",
    "ECA5"
  ),
  
    new ("Calories",
    "ECAD"
  ),
  
    new ("POI",
    "ECAF"
  ),
  
    new ("BandBattery0",
    "ECB9"
  ),
  
    new ("BandBattery1",
    "ECBA"
  ),
  
    new ("BandBattery2",
    "ECBB"
  ),
  
    new ("BandBattery3",
    "ECBC"
  ),
  
    new ("BandBattery4",
    "ECBD"
  ),
  
    new ("BandBattery5",
    "ECBE"
  ),
  
    new ("BandBattery6",
    "ECBF"
  ),
  
    new ("Unit",
    "ECC6"
  ),
  
    new ("AddTo",
    "ECC8"
  ),
  
    new ("RemoveFrom",
    "ECC9"
  ),
  
    new ("RadioBtnOff",
    "ECCA"
  ),
  
    new ("RadioBtnOn",
    "ECCB"
  ),
  
    new ("RadioBullet2",
    "ECCC"
  ),
  
    new ("ExploreContent",
    "ECCD"
  ),
  
    new ("Blocked2",
    "ECE4"
  ),
  
    new ("QRCode",
    "ED14"
  ),
  
    new ("Feedback",
    "ED15"
  ),
  
    new ("Hide",
    "ED1A"
  ),
  
    new ("Subtitles",
    "ED1E"
  ),
  
    new ("SubtitlesAudio",
    "ED1F"
  ),
  
    new ("OpenFolderHorizontal",
    "ED25"
  ),
  
    new ("CalendarMirrored",
    "ED28"
  ),
  
    new ("SkipBack10",
    "ED3C"
  ),
  
    new ("SkipForward30",
    "ED3D"
  ),
  
    new ("TreeFolderFolder",
    "ED41"
  ),
  
    new ("TreeFolderFolderFill",
    "ED42"
  ),
  
    new ("TreeFolderFolderOpen",
    "ED43"
  ),
  
    new ("TreeFolderFolderOpenFill",
    "ED44"
  ),
  
    new ("EmojiTabSmilesAnimals",
    "ED54"
  ),
  
    new ("EmojiTabFoodPlants",
    "ED56"
  ),
  
    new ("EmojiTabTransitPlaces",
    "ED57"
  ),
  
    new ("EmojiTabFavorites",
    "ED5A"
  ),
  
    new ("EmojiSwatch",
    "ED5B"
  ),
  
    new ("Ruler",
    "ED5E"
  ),
  
    new ("HardDrive",
    "EDA2"
  ),
  
    new ("CircleRingBadge12",
    "EDAF"
  ),
  
    new ("MailBadge12",
    "EDB3"
  ),
  
    new ("PauseBadge12",
    "EDB4"
  ),
  
    new ("PlayBadge12",
    "EDB5"
  ),
  
    new ("CaretLeft8",
    "EDD5"
  ),
  
    new ("CaretRight8",
    "EDD6"
  ),
  
    new ("CaretUp8",
    "EDD7"
  ),
  
    new ("CaretDown8",
    "EDD8"
  ),
  
    new ("CaretLeftSolid8",
    "EDD9"
  ),
  
    new ("CaretRightSolid8",
    "EDDA"
  ),
  
    new ("CaretUpSolid8",
    "EDDB"
  ),
  
    new ("CaretDownSolid8",
    "EDDC"
  ),
  
    new ("Strikethrough",
    "EDE0"
  ),
  
    new ("Export",
    "EDE1"
  ),
  
    new ("CalligraphyPen",
    "EDFB"
  ),
  
    new ("ReplyMirrored",
    "EE35"
  ),
  
    new ("TaskViewSettings",
    "EE40"
  ),
  
    new ("Play36",
    "EE4A"
  ),
  
    new ("SettingsBattery",
    "EE63"
  ),
  
    new ("DateTimeMirrored",
    "EE93"
  ),
  
    new ("ChromeCloseContrast",
    "EF2C"
  ),
  
    new ("ChromeMinimizeContrast",
    "EF2D"
  ),
  
    new ("ChromeMaximizeContrast",
    "EF2E"
  ),
  
    new ("ChromeRestoreContrast",
    "EF2F"
  ),
  
    new ("TrafficLight",
    "EF31"
  ),
  
    new ("Replay",
    "EF3B"
  ),
  
    new ("Eyedropper",
    "EF3C"
  ),
  
    new ("LandscapeOrientation",
    "EF6B"
  ),
  
    new ("Flow",
    "EF90"
  ),
  
    new ("Speech",
    "EFA9"
  ),
  
    new ("Relationship",
    "F003"
  ),
  
    new ("ZipFolder",
    "F012"
  ),
  
    new ("CaretSolidLeft",
    "F08D"
  ),
  
    new ("CaretSolidDown",
    "F08E"
  ),
  
    new ("CaretSolidRight",
    "F08F"
  ),
  
    new ("CaretSolidUp",
    "F090"
  ),
  
    new ("ArrowUp8",
    "F0AD"
  ),
  
    new ("ArrowDown8",
    "F0AE"
  ),
  
    new ("ArrowRight8",
    "F0AF"
  ),
  
    new ("ArrowLeft8",
    "F0B0"
  ),
  
    new ("ChecklistMirrored",
    "F0B5"
  ),
  
    new ("QuietHoursBadge12",
    "F0CE"
  ),
  
    new ("BackMirrored",
    "F0D2"
  ),
  
    new ("ForwardMirrored",
    "F0D3"
  ),
  
    new ("ChromeBackContrast",
    "F0D5"
  ),
  
    new ("ChromeBackContrastMirrored",
    "F0D6"
  ),
  
    new ("ChromeBackToWindowContrast",
    "F0D7"
  ),
  
    new ("ChromeFullScreenContrast",
    "F0D8"
  ),
  
    new ("GridView",
    "F0E2"
  ),
  
    new ("ClipboardList",
    "F0E3"
  ),
  
    new ("OutlineQuarterStarLeft",
    "F0E5"
  ),
  
    new ("OutlineHalfStarLeft",
    "F0E7"
  ),
  
    new ("OutlineThreeQuarterStarLeft",
    "F0E9"
  ),
  
    new ("ChromeAnnotateContrast",
    "F0F9"
  ),
  
    new ("LeftStick",
    "F108"
  ),
  
    new ("RightStick",
    "F109"
  ),
  
    new ("PaginationDotOutline10",
    "F126"
  ),
  
    new ("PaginationDotSolid10",
    "F127"
  ),
  
    new ("FolderHorizontal",
    "F12B"
  ),
  
    new ("MicrophoneListening",
    "F12E"
  ),
  
    new ("StatusCircleBlock",
    "F140"
  ),
  
    new ("StatusCircleBlock2",
    "F141"
  ),
  
    new ("ExploreContentSingle",
    "F164"
  ),
  
    new ("InfoSolid",
    "F167"
  ),
  
    new ("GroupList",
    "F168"
  ),
  
    new ("Checkbox14",
    "F16B"
  ),
  
    new ("CheckboxComposite14",
    "F16C"
  ),
  
    new ("ToggleLeft",
    "F19E"
  ),
  
    new ("ToggleRight",
    "F19F"
  ),
  
    new ("WindowsInsider",
    "F1AD"
  ),
  
    new ("ChromeSwitch",
    "F1CB"
  ),
  
    new ("ChromeSwitchContast",
    "F1CC"
  ),
  
    new ("KeyboardLeftAligned",
    "F20C"
  ),
  
    new ("Bullseye",
    "F272"
  ),
  
    new ("DocumentApproval",
    "F28B"
  ),
  
    new ("ColorSolid",
    "F354"
  ),
  
    new ("SignOut",
    "F3B1"
  ),
  
    new ("DeclineCall",
    "F405"
  ),
  
    new ("ClippingTool",
    "F406"
  ),
  
    new ("CopyTo",
    "F413"
  ),
  
    new ("MobWifiWarning4",
    "F476"
  ),
  
    new ("GIF",
    "F4A9"
  ),
  
    new ("Sticker2",
    "F4AA"
  ),
  
    new ("Earbud",
    "F4C0"
  ),
  
    new ("PageMirrored",
    "F56E"
  ),
  
    new ("LandscapeOrientationMirrored",
    "F56F"
  ),
  
    new ("DuplexPortraitOneSided",
    "F584"
  ),
  
    new ("PlaySolid",
    "F5B0"
  ),
  
    new ("RepeatOff",
    "F5E7"
  ),
  
    new ("Set",
    "F5ED"
  ),
  
    new ("SetSolid",
    "F5EE"
  ),
  
    new ("CircleShapeSolid",
    "F63C"
  ),
  
    new ("WebSearch",
    "F6FA"
  ),
  
    new ("VoiceCall",
    "F715"
  ),
  
    new ("ReturnToCall",
    "F71A"
  ),
  
    new ("StartPresenting",
    "F71C"
  ),
  
    new ("StopPresenting",
    "F71D"
  ),
  
    new ("SetHistoryStatus",
    "F738"
  ),
  
    new ("OneHandedLeft20",
    "F73F"
  ),
  
    new ("Split20",
    "F740"
  ),
  
    new ("Full20",
    "F741"
  ),
  
    new ("ChevronLeft20",
    "F743"
  ),
  
    new ("ChevronLeft32",
    "F744"
  ),
  
    new ("ChevronRight20",
    "F745"
  ),
  
    new ("ChevronRight32",
    "F746"
  ),
  
    new ("Event12",
    "F763"
  ),
  
    new ("MicOff2",
    "F781"
  ),
  
    new ("CancelMedium",
    "F78A"
  ),
  
    new ("SearchMedium",
    "F78B"
  ),
  
    new ("AcceptMedium",
    "F78C"
  ),
  
    new ("RevealPasswordMedium",
    "F78D"
  ),
  
    new ("DeleteLines",
    "F7AF"
  ),
  
    new ("DeleteLinesFill",
    "F7B0"
  ),
  
    new ("Eject",
    "F847"
  ),
  
    new ("Spelling",
    "F87B"
  ),
  
    new ("AddBold",
    "F8AA"
  ),
  
    new ("SubtractBold",
    "F8AB"
  ),
  
    new ("BackSolidBold",
    "F8AC"
  ),
  
    new ("ForwardSolidBold",
    "F8AD"
  ),
  
    new ("PauseBold",
    "F8AE"
  ),
  
    new ("ClickSolid",
    "F8AF"
  ),
  
    new ("SettingsSolid",
    "F8B0"
  ),
  
    new ("MicrophoneSolidBold",
    "F8B1"
  ),
  
    new ("SpeechSolidBold",
    "F8B2")
             ];
}

