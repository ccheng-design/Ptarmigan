using System;
using System.IO;
using System.Text;
using SD = System.Drawing;

using Rhino;
using Grasshopper.Kernel;

namespace RhinoCodePlatform.Rhino3D.Projects.Plugin.GH
{
  public sealed class AssemblyInfo : GH_AssemblyInfo
  {
    static readonly string s_assemblyIconData = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAARpJREFUSEvtlGFxAkEMhU9CJVRCHRQJlYCEOgAn4AAcFAetBByAA5pv975Oegd3PfhVhjfzhiSb5O1NsjR3hefga/CpeNV+qWYBcWI5B5BjPOf3sAyegrPiVXtXzYL3ILGcA8gxDjfBsxgT8BwiJhRYB79a+y3Yw5jANrgPEidXKEAdjbvnPxgTwLYZYiILwKsFbHwMdoVvFmBrLCR2CIosYI/JAvlmNhT6ki9k5XsYEnB4+ZbmKcAGsUkX38KQgGer4EdrdwX0L8Im8+L9FuBm+Jnkg8kC/DpUBXITX/RkAQt56ovWdt/ZGoYHGCBn58QHwa19qZCGFuWGAP+zmn8XAIgwA74mrxrFeTvwbUgcO//DPvDA/0HTfANH2owcYXh1QAAAAABJRU5ErkJggg==";
    static readonly string s_categoryIconData = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAAN9JREFUOE/t0iGzAVEYh/ENgigIonA/gCAIgnCDDyCIouBDCIIPcaMgiLobBU3R3CYwIwiCwPO4zg6zZ6SN/jO/2fW+Z985u0eSVypooIgWSjDWav+3aU9lC8/p4Q9VXOEic4Z1E3on7PAyJDbAXTnA3wWE3hcO6CJNbEATK1zgQ6Hn1bU+kyY2wNoc1tt47h3RQZrYgBF+8Is+Qk/uLHzoe2IDplhgjTFCr45MYgOW2GALh4We10x8H49nABf50fb4ftTc8tsBHtME7mII/zS+u4vdzQweqzWvn+SXJLkBXXNADAx/AyEAAAAASUVORK5CYII=";

    public static readonly SD.Bitmap PluginIcon = default;
    public static readonly SD.Bitmap PluginCategoryIcon = default;

    static AssemblyInfo()
    {
      if (!s_assemblyIconData.Contains("ASSEMBLY-ICON"))
      {
        using (var aicon = new MemoryStream(Convert.FromBase64String(s_assemblyIconData)))
          PluginIcon = new SD.Bitmap(aicon);
      }

      if (!s_categoryIconData.Contains("ASSEMBLY-CATEGORY-ICON"))
      {
        using (var cicon = new MemoryStream(Convert.FromBase64String(s_categoryIconData)))
          PluginCategoryIcon = new SD.Bitmap(cicon);
      }
    }

    public override Guid Id { get; } = new Guid("99a1eda8-81c6-4967-b683-d7dcd01e92ab");

    public override string AssemblyName { get; } = "UAP.Components";
    public override string AssemblyVersion { get; } = "0.3.8.9033";
    public override string AssemblyDescription { get; } = "This UAP plugin is designed to enhance workflow efficiency by addressing missing features in Rhino. It simplifies and consolidates common Grasshopper scripts into Rhino commands, making them more accessible to team members. The plugin introduces improvements to layouts, selection tools, and object naming functionality, offering a more streamlined and user-friendly experience. These upgrades aim to enhance productivity and provide a smoother design process within Rhino.";
    public override string AuthorName { get; } = "Clark Cheng";
    public override string AuthorContact { get; } = "clark.cheng@uapcompany.com";
    public override GH_LibraryLicense AssemblyLicense { get; } = GH_LibraryLicense.unset;
    public override SD.Bitmap AssemblyIcon { get; } = PluginIcon;
  }

  public class ProjectComponentPlugin : GH_AssemblyPriority
  {
    static readonly Guid s_projectId = new Guid("99a1eda8-81c6-4967-b683-d7dcd01e92ab");
    static readonly string s_projectData = "ew0KICAiaG9zdCI6IHsNCiAgICAibmFtZSI6ICJSaGlubzNEIiwNCiAgICAidmVyc2lvbiI6ICI4LjExLjI0MjU0XHUwMDJCMTUwMDEiLA0KICAgICJvcyI6ICJ3aW5kb3dzIiwNCiAgICAiYXJjaCI6ICJ4NjQiDQogIH0sDQogICJpZCI6ICI5OWExZWRhOC04MWM2LTQ5NjctYjY4My1kN2RjZDAxZTkyYWIiLA0KICAiaWRlbnRpdHkiOiB7DQogICAgIm5hbWUiOiAiVUFQIiwNCiAgICAidmVyc2lvbiI6ICIwLjMuOC1iZXRhIiwNCiAgICAicHVibGlzaGVyIjogew0KICAgICAgImVtYWlsIjogImNsYXJrLmNoZW5nQHVhcGNvbXBhbnkuY29tIiwNCiAgICAgICJuYW1lIjogIkNsYXJrIENoZW5nIiwNCiAgICAgICJjb21wYW55IjogIlVBUCINCiAgICB9LA0KICAgICJkZXNjcmlwdGlvbiI6ICJUaGlzIFVBUCBwbHVnaW4gaXMgZGVzaWduZWQgdG8gZW5oYW5jZSB3b3JrZmxvdyBlZmZpY2llbmN5IGJ5IGFkZHJlc3NpbmcgbWlzc2luZyBmZWF0dXJlcyBpbiBSaGluby4gSXQgc2ltcGxpZmllcyBhbmQgY29uc29saWRhdGVzIGNvbW1vbiBHcmFzc2hvcHBlciBzY3JpcHRzIGludG8gUmhpbm8gY29tbWFuZHMsIG1ha2luZyB0aGVtIG1vcmUgYWNjZXNzaWJsZSB0byB0ZWFtIG1lbWJlcnMuIFRoZSBwbHVnaW4gaW50cm9kdWNlcyBpbXByb3ZlbWVudHMgdG8gbGF5b3V0cywgc2VsZWN0aW9uIHRvb2xzLCBhbmQgb2JqZWN0IG5hbWluZyBmdW5jdGlvbmFsaXR5LCBvZmZlcmluZyBhIG1vcmUgc3RyZWFtbGluZWQgYW5kIHVzZXItZnJpZW5kbHkgZXhwZXJpZW5jZS4gVGhlc2UgdXBncmFkZXMgYWltIHRvIGVuaGFuY2UgcHJvZHVjdGl2aXR5IGFuZCBwcm92aWRlIGEgc21vb3RoZXIgZGVzaWduIHByb2Nlc3Mgd2l0aGluIFJoaW5vLiIsDQogICAgImNvcHlyaWdodCI6ICJDb3B5cmlnaHQgXHUwMEE5IDIwMjQiLA0KICAgICJsaWNlbnNlIjogIk1JVCIsDQogICAgInVybCI6ICJodHRwczovL2dpdGh1Yi5jb20vY2NoZW5nLWRlc2lnbi9VQVAtUGx1Z2lucyIsDQogICAgImltYWdlIjogew0KICAgICAgImxpZ2h0Ijogew0KICAgICAgICAidHlwZSI6ICJzdmciLA0KICAgICAgICAiZGF0YSI6ICJQSE4yWnlCMlpYSnphVzl1UFNJeExqQWlJSGh0Ykc1elBTSm9kSFJ3T2k4dmQzZDNMbmN6TG05eVp5OHlNREF3TDNOMlp5SUtJSGRwWkhSb1BTSXhNREF3TGpBd01EQXdNSEIwSWlCb1pXbG5hSFE5SWpjMk9TNHdNREF3TURCd2RDSWdkbWxsZDBKdmVEMGlNQ0F3SURFd01EQXVNREF3TURBd0lEYzJPUzR3TURBd01EQWlDaUJ3Y21WelpYSjJaVUZ6Y0dWamRGSmhkR2x2UFNKNFRXbGtXVTFwWkNCdFpXVjBJajRLQ2p4bklIUnlZVzV6Wm05eWJUMGlkSEpoYm5Oc1lYUmxLREF1TURBd01EQXdMRGMyT1M0d01EQXdNREFwSUhOallXeGxLREF1TVRBd01EQXdMQzB3TGpFd01EQXdNQ2tpQ21acGJHdzlJaU13TURBd01EQWlJSE4wY205clpUMGlibTl1WlNJXHUwMDJCQ2p4d1lYUm9JR1E5SWsweE1ERTBJRFkyTmpjZ1l5MHpJQzA0SUMweklDMHhNRGM0SUMweElDMHlNemM0SURNZ0xURTNOakFnTnlBdE1qTTROQ0F4TmlBdE1qUTBOQ0F6TndvdE1qVXhJREV5TlNBdE5EUXdJREkzTlNBdE5Ua3dJRFF5T1NBdE5ESTRJREV5TVRrZ0xUUXpNeUF4TmpVd0lDMHhNQ0F4TlRZZ01UVXpJREl6TnlBek1UTWdNamN4SURVek5Rb3pNaUF5TURVZ016WWdOVFU0SURNeklESTNNelVnYkMweklESXhOakFnTFRNNU55QXpJQzB6T1RnZ01pQXdJQzB5TWpZM0lHTXdJQzB4TkRJeklDMDBJQzB5TXpFd0lDMHhNQW90TWpNNE1TQXRNVFlnTFRFNE1TQXROakFnTFRJM01TQXRNVFUzSUMwek1Ua2dMVFV5SUMweU5TQXROamdnTFRJNElDMHhOVGdnTFRJNElDMDRPU0F3SUMweE1EWWdNd290TVRVeElESTNJQzAyTlNBek5DQXRNVEEzSURnMElDMHhNellnTVRZeUlHd3RNak1nTmpFZ0xUVWdNak0zTUNBdE5TQXlNemN3SUMwek9UZ2dNeUJqTFRNeU9TQXlJQzB6T1RnS01DQXROREF6SUMweE1Yb2lMejRLUEhCaGRHZ2daRDBpVFRRME5qVWdOalkyT0NCakxUTWdMVEV3SUMwM05UZ2dMVFUxTkRnZ0xUYzJPQ0F0TlRZek15QXRNaUF0TWpBZ05TQXRNakFnTXpnd0lDMHlNeUJzTXpneENpMHlJRFlnTXpjZ1l6TWdNakVnTXpNZ01qazVJRFkySURZeE9DQXpNeUF6TVRrZ05qTWdOVGt3SURZMklEWXdNeUJzTkNBeU1pQXpOVEVnTUNCak1qY3hJREFnTXpVeElDMHpDak0xTkNBdE1USWdNaUF0TnlBek15QXRNamM0SURjd0lDMDJNRE1nTXpZZ0xUTXlOQ0EyT0NBdE5qQTNJRGN4SUMwMk1qY2diRFlnTFRNNElETTROQ0F3SURNNE5DQXdJREFLTWpRZ1l6QWdNeklnTFRjeU9DQTFOakUzSUMwM016VWdOVFl6TkNBdE15QTVJQzB4TVRVZ01USWdMVFV4TUNBeE1pQXRNemsySURBZ0xUVXdOeUF0TXlBdE5URXdJQzB4TW5vS2JUYzFOU0F0TXpZek1DQmpNQ0F0TlNBdE1USXlJQzA0SUMweU56QWdMVGdnYkMweU56RWdNQ0EySURNeklHTXpJREUzSURZeUlEVTRNeUF4TXpBZ01USTFOeUEyT1NBMk56UUtNVEk1SURFeU16WWdNVE15SURFeU5UQWdOQ0F4TnlBeU5qWWdMVEkwTVRnZ01qY3pJQzB5TlRNeWVpSXZQZ284Y0dGMGFDQmtQU0pOTmpjM05TQTJOalk0SUdNdE15QXROeUF0TkNBdE1USTRNaUF0TXlBdE1qZ3pNeUJzTXlBdE1qZ3lNQ0F6T1RnZ0xUTWdNemszSUMweUlEQWdNVEUxTUFvd0lERXhOVEFnTnprZ01DQmpNVEl3SURBZ016STBJREU1SURRek1TQTBNQ0F6TWpNZ05qSWdOVFF3SURFNU55QTJPVGtnTkRNMElERTBNaUF5TVRBZ01Ua3dJRFF3TVNBeU1USUtPRFV4SURFNUlETTNNU0F6SURnek5DQXRNellnTVRBMk5TQXROamdnTkRBMElDMHpNVEVnTnpVMElDMDJNRFVnT0RjMElDMHlNRFlnT0RRZ0xUUTBNU0F4TURZZ0xURXhOak1LTVRBMklDMHpNVGNnTUNBdE5EQTVJQzB6SUMwME1USWdMVEV5ZWlCdE1URXhOaUF0TnpZeklHTXpOeUF0TnlBNE5DQXRNaklnTVRBMUlDMHpNeUE1TkNBdE5Ea2dNVFk1Q2kweE5qY2dNVGs0SUMwek1USWdORGNnTFRJek5TQTBOeUF0T1RFeklEQWdMVEV4TlRBZ0xUZ2dMVFF4SUMweU9DQXRNVEF6SUMwME15QXRNVE0ySUMwM05pQXRNVFl6Q2kweE9Ua2dMVEl5TXlBdE5EVTRJQzB5TWpRZ2JDMHhNak1nTUNBd0lEazBNU0F3SURrME1TQXhNamdnTFRjZ1l6Y3dJQzB6SURFMU55QXRNVE1nTVRreklDMHlNSG9pTHo0S1BDOW5QZ284TDNOMlp6NEsiDQogICAgICB9LA0KICAgICAgInByb2plY3RJY29uIjogew0KICAgICAgICAibGlnaHQiOiB7DQogICAgICAgICAgImJ5dGVzIjogImlWQk9SdzBLR2dvQUFBQU5TVWhFVWdBQUFCZ0FBQUFZQ0FZQUFBRGdkejM0QUFBQUJHZEJUVUVBQUxHUEMveGhCUUFBQUFsd1NGbHpBQUFPd2dBQURzSUJGU2hLZ0FBQUFScEpSRUZVU0V2dGxHRnhBa0VNaFU5Q0pWUkNIUlFKbFlDRU9nQW40QUFjRkFldEJCeUFBNXB2OTc1T2VnZDNQZmhWaGpmemhpU2I1TzFOc2pSM2hlZmdhL0NwZU5WXHUwMDJCcVdZQmNXSTVCNUJqUE9mM3NBeWVnclBpVlh0WHpZTDNJTEdjQThneERqZkJzeGdUOEJ3aUpoUllCNzlhXHUwMDJCeTNZdzVqQU5yZ1BFaWRYS0VBZGpidm5QeGdUd0xZWllpSUx3S3NGYkh3TWRvVnZGbUJyTENSMkNJb3NZSS9KQXZsbU5oVDZraTlrNVhzWUVuQjRcdTAwMkJaYm1LY0FHc1VrWDM4S1FnR2VyNEVkcmR3WDBMOEltOFx1MDAyQkw5RnVCbVx1MDAyQkpua2c4a0MvRHBVQlhJVFgvUmtBUXQ1Nm92V2R0L1pHb1lIR0NCbjU4UUh3YTE5cVpDR0Z1V0dBUFx1MDAyQnptbjhYQUlnd0E3NG1yeHJGZVR2d2JVZ2NPLy9EUHZEQS8wSFRmQU5IMm93Y1lYaDFRQUFBQUFCSlJVNUVya0pnZ2c9PSIsDQogICAgICAgICAgIndpZHRoIjogMjQsDQogICAgICAgICAgImhlaWdodCI6IDI0DQogICAgICAgIH0sDQogICAgICAgICJkYXJrIjogew0KICAgICAgICAgICJieXRlcyI6ICJpVkJPUncwS0dnb0FBQUFOU1VoRVVnQUFBQmdBQUFBWUNBWUFBQURnZHozNEFBQUFCR2RCVFVFQUFMR1BDL3hoQlFBQUFBbHdTRmx6QUFBT3dnQUFEc0lCRlNoS2dBQUFBQmhKUkVGVVNFdnR3UUVCQUFBQWdpRC9yNjRoUUFBQVhBMEpHQUFCMEFKYlhnQUFBQUJKUlU1RXJrSmdnZz09IiwNCiAgICAgICAgICAid2lkdGgiOiAyNCwNCiAgICAgICAgICAiaGVpZ2h0IjogMjQNCiAgICAgICAgfSwNCiAgICAgICAgImljb0RhdGEiOiAiQUFBQkFBRUFHQmdBQUFFQUlBQ2JBUUFBRmdBQUFJbFFUa2NOQ2hvS0FBQUFEVWxJUkZJQUFBQVlBQUFBR0FnR0FBQUE0SGM5XHUwMDJCQUFBQUFSblFVMUJBQUN4and2OFlRVUFBQUFKY0VoWmN3QUFEc0lBQUE3Q0FSVW9Tb0FBQUFFOVNVUkJWRWhMN1pTeFRZTXhFSVYvWUFIWWdJNFNPa3BnQXphQURVZ21DS1BBQkxBQmJBQnNFRHBLNk9qSVx1MDAyQjJ3L3kzR01aSmVSOGtsUFBqdk9uYzkzdjZldDV5Q05jQ3lkU2ovU3IzUWhIVWxmRWh4SzV4TDd2QWZPcEJPSjlYTC9CdmZTbjNRWlp0Rlx1MDAyQmpXWmdKckZXN2dIMmVCMDlTWm45TlBaQUJvWlQxenhLSDlKMVVtQWtBRTQvbzdrV3pEeEkzQUxrQTR4bXNJeG1Nd1A0VG1ObUpBQkZ4d0VGYm1YUXBEZUFIYjRuMFcwdHl1SUhlZ1BVVjlMSzRFVmFTR1JJUFFLakdkQ1NidDM2dEhRUW5jUzZheldjd1kxRUxWcnduZHhLWEdHbUZZQXZzc1pyT1BESk5cdTAwMkI2N3hYOEI2anQyZ0N0cEhzMFx1MDAyQnlnRHVZVHJrTHBwcmF4U1BcdTAwMkIzOW1RWFJsVU1LcEtZN2ZGSnpiQ2ZQeVhXTFx1MDAyQkZzMzhGalVEN3FYUkVJUjNoSkdUdWh2NE13RmRRRHZET1EzQWZuNXp4anQyYkEvVHRBSTJ2ajdcdTAwMkJ4dWp5U3dBQUFBQkpSVTVFcmtKZ2dnPT0iDQogICAgICB9LA0KICAgICAgImNhdGVnb3J5SWNvbiI6IHsNCiAgICAgICAgImxpZ2h0Ijogew0KICAgICAgICAgICJieXRlcyI6ICJpVkJPUncwS0dnb0FBQUFOU1VoRVVnQUFBQkFBQUFBUUNBWUFBQUFmOC85aEFBQUFCR2RCVFVFQUFMR1BDL3hoQlFBQUFBbHdTRmx6QUFBT3dnQUFEc0lCRlNoS2dBQUFBTjlKUkVGVU9FL3QwaUd6QVZFWWgvRU5naWdJb25BL2dDQUlnbkNERHlDSW91QkRDSUlQY2FNZ2lMb2JCVTNSM0NZd0l3aUN3UE80emc2elo2U04vak8vMmZXXHUwMDJCWjk4NXUwZVNWeXBvb0lnV1NqRFdhdlx1MDAyQjNhVTlsQzgvcDRROVZYT0VpYzRaMUUzb243UEF5SkRiQVhUbkEzd1dFM2hjTzZDSk5iRUFUSzF6Z1E2SG4xYlVcdTAwMkJreVkyd05vYzF0dDQ3aDNSUVpyWWdCRlx1MDAyQjhJc1x1MDAyQlFrL3VMSHpvZTJJRHBsaGdqVEZDcjQ1TVlnT1cyR0FMaDRXZTEweDhINDluQUJmNTBmYjRmdFRjOHRzQkh0TUU3bUlJL3pTXHUwMDJCdTR2ZHpRd2Vxeld2blx1MDAyQlNYSkxrQlhYTkFEQXgvQXlFQUFBQUFTVVZPUks1Q1lJST0iLA0KICAgICAgICAgICJ3aWR0aCI6IDE2LA0KICAgICAgICAgICJoZWlnaHQiOiAxNg0KICAgICAgICB9LA0KICAgICAgICAiZGFyayI6IHsNCiAgICAgICAgICAiYnl0ZXMiOiAiaVZCT1J3MEtHZ29BQUFBTlNVaEVVZ0FBQUJBQUFBQVFDQVlBQUFBZjgvOWhBQUFBQkdkQlRVRUFBTEdQQy94aEJRQUFBQWx3U0ZsekFBQU93Z0FBRHNJQkZTaEtnQUFBQUJOSlJFRlVPRTlqR0FXallCU01BakJnWUFBQUJCQUFBYWRFZkdNQUFBQUFTVVZPUks1Q1lJST0iLA0KICAgICAgICAgICJ3aWR0aCI6IDE2LA0KICAgICAgICAgICJoZWlnaHQiOiAxNg0KICAgICAgICB9DQogICAgICB9DQogICAgfQ0KICB9LA0KICAic2V0dGluZ3MiOiB7DQogICAgImJ1aWxkUGF0aCI6ICJmaWxlOi8vL0M6L1VzZXJzL2NsYXJrLmNoZW5nL09uZURyaXZlIC0gVUFQIENvbXBhbnkvRGVza3RvcC9fQ09NTU9OUy9VQVAtUGx1Z2lucy8wMV9SSElOTy9idWlsZC9yaDgiLA0KICAgICJidWlsZFRhcmdldCI6IHsNCiAgICAgICJob3N0Ijogew0KICAgICAgICAibmFtZSI6ICJSaGlubzNEIiwNCiAgICAgICAgInZlcnNpb24iOiAiOCINCiAgICAgIH0sDQogICAgICAidGl0bGUiOiAiUmhpbm8zRCAoOC4qKSIsDQogICAgICAic2x1ZyI6ICJyaDgiDQogICAgfSwNCiAgICAicHVibGlzaFRhcmdldCI6IHsNCiAgICAgICJ0aXRsZSI6ICJNY05lZWwgWWFrIFNlcnZlciINCiAgICB9DQogIH0sDQogICJjb2RlcyI6IFtdDQp9";
    static readonly dynamic s_projectServer = default;
    static readonly object s_project = default;

    static ProjectComponentPlugin()
    {
      s_projectServer = ProjectInterop.GetProjectServer();
      if (s_projectServer is null)
      {
        RhinoApp.WriteLine($"Error loading Grasshopper plugin. Missing Rhino3D platform");
        return;
      }

      // get project
      dynamic dctx = ProjectInterop.CreateInvokeContext();
      dctx.Inputs["projectAssembly"] = typeof(ProjectComponentPlugin).Assembly;
      dctx.Inputs["projectId"] = s_projectId;
      dctx.Inputs["projectData"] = s_projectData;

      object project = default;
      if (s_projectServer.TryInvoke("plugins/v1/deserialize", dctx)
            && dctx.Outputs.TryGet("project", out project))
      {
        // server reports errors
        s_project = project;
      }
    }

    public override GH_LoadingInstruction PriorityLoad()
    {
      if (AssemblyInfo.PluginCategoryIcon is SD.Bitmap icon)
      {
        Grasshopper.Instances.ComponentServer.AddCategoryIcon("UAP", icon);
      }
      Grasshopper.Instances.ComponentServer.AddCategorySymbolName("UAP", "UAP"[0]);

      return GH_LoadingInstruction.Proceed;
    }

    public static bool TryCreateScript(GH_Component ghcomponent, string serialized, out object script)
    {
      script = default;

      if (s_projectServer is null) return false;

      dynamic dctx = ProjectInterop.CreateInvokeContext();
      dctx.Inputs["component"] = ghcomponent;
      dctx.Inputs["project"] = s_project;
      dctx.Inputs["scriptData"] = serialized;

      if (s_projectServer.TryInvoke("plugins/v1/gh/deserialize", dctx))
      {
        return dctx.Outputs.TryGet("script", out script);
      }

      return false;
    }

    public static void DisposeScript(GH_Component ghcomponent, object script)
    {
      if (script is null)
        return;

      dynamic dctx = ProjectInterop.CreateInvokeContext();
      dctx.Inputs["component"] = ghcomponent;
      dctx.Inputs["project"] = s_project;
      dctx.Inputs["script"] = script;

      if (!s_projectServer.TryInvoke("plugins/v1/gh/dispose", dctx))
        throw new Exception("Error disposing Grasshopper script component");
    }
  }
}
