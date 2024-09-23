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
    public override string AssemblyDescription { get; } = "";
    public override string AuthorName { get; } = "Clark Cheng";
    public override string AuthorContact { get; } = "clark.cheng@uapcompany.com";
    public override GH_LibraryLicense AssemblyLicense { get; } = GH_LibraryLicense.unset;
    public override SD.Bitmap AssemblyIcon { get; } = PluginIcon;
  }

  public class ProjectComponentPlugin : GH_AssemblyPriority
  {
    static readonly Guid s_projectId = new Guid("99a1eda8-81c6-4967-b683-d7dcd01e92ab");
    static readonly string s_projectData = "ew0KICAiaG9zdCI6IHsNCiAgICAibmFtZSI6ICJSaGlubzNEIiwNCiAgICAidmVyc2lvbiI6ICI4LjExLjI0MjU0XHUwMDJCMTUwMDEiLA0KICAgICJvcyI6ICJ3aW5kb3dzIiwNCiAgICAiYXJjaCI6ICJ4NjQiDQogIH0sDQogICJpZCI6ICI5OWExZWRhOC04MWM2LTQ5NjctYjY4My1kN2RjZDAxZTkyYWIiLA0KICAiaWRlbnRpdHkiOiB7DQogICAgIm5hbWUiOiAiVUFQIiwNCiAgICAidmVyc2lvbiI6ICIwLjMuOC1iZXRhIiwNCiAgICAicHVibGlzaGVyIjogew0KICAgICAgImVtYWlsIjogImNsYXJrLmNoZW5nQHVhcGNvbXBhbnkuY29tIiwNCiAgICAgICJuYW1lIjogIkNsYXJrIENoZW5nIiwNCiAgICAgICJjb21wYW55IjogIlVBUCINCiAgICB9LA0KICAgICJjb3B5cmlnaHQiOiAiQ29weXJpZ2h0IFx1MDBBOSAyMDI0IiwNCiAgICAiaW1hZ2UiOiB7DQogICAgICAibGlnaHQiOiB7DQogICAgICAgICJ0eXBlIjogInN2ZyIsDQogICAgICAgICJkYXRhIjogIlBITjJaeUIyWlhKemFXOXVQU0l4TGpBaUlIaHRiRzV6UFNKb2RIUndPaTh2ZDNkM0xuY3pMbTl5Wnk4eU1EQXdMM04yWnlJS0lIZHBaSFJvUFNJeE1EQXdMakF3TURBd01IQjBJaUJvWldsbmFIUTlJamMyT1M0d01EQXdNREJ3ZENJZ2RtbGxkMEp2ZUQwaU1DQXdJREV3TURBdU1EQXdNREF3SURjMk9TNHdNREF3TURBaUNpQndjbVZ6WlhKMlpVRnpjR1ZqZEZKaGRHbHZQU0o0VFdsa1dVMXBaQ0J0WldWMElqNEtDanhuSUhSeVlXNXpabTl5YlQwaWRISmhibk5zWVhSbEtEQXVNREF3TURBd0xEYzJPUzR3TURBd01EQXBJSE5qWVd4bEtEQXVNVEF3TURBd0xDMHdMakV3TURBd01Da2lDbVpwYkd3OUlpTXdNREF3TURBaUlITjBjbTlyWlQwaWJtOXVaU0lcdTAwMkJDanh3WVhSb0lHUTlJazB4TURFMElEWTJOamNnWXkweklDMDRJQzB6SUMweE1EYzRJQzB4SUMweU16YzRJRE1nTFRFM05qQWdOeUF0TWpNNE5DQXhOaUF0TWpRME5DQXpOd290TWpVeElERXlOU0F0TkRRd0lESTNOU0F0TlRrd0lEUXlPU0F0TkRJNElERXlNVGtnTFRRek15QXhOalV3SUMweE1DQXhOVFlnTVRVeklESXpOeUF6TVRNZ01qY3hJRFV6TlFvek1pQXlNRFVnTXpZZ05UVTRJRE16SURJM016VWdiQzB6SURJeE5qQWdMVE01TnlBeklDMHpPVGdnTWlBd0lDMHlNalkzSUdNd0lDMHhOREl6SUMwMElDMHlNekV3SUMweE1Bb3RNak00TVNBdE1UWWdMVEU0TVNBdE5qQWdMVEkzTVNBdE1UVTNJQzB6TVRrZ0xUVXlJQzB5TlNBdE5qZ2dMVEk0SUMweE5UZ2dMVEk0SUMwNE9TQXdJQzB4TURZZ013b3RNVFV4SURJM0lDMDJOU0F6TkNBdE1UQTNJRGcwSUMweE16WWdNVFl5SUd3dE1qTWdOakVnTFRVZ01qTTNNQ0F0TlNBeU16Y3dJQzB6T1RnZ015QmpMVE15T1NBeUlDMHpPVGdLTUNBdE5EQXpJQzB4TVhvaUx6NEtQSEJoZEdnZ1pEMGlUVFEwTmpVZ05qWTJPQ0JqTFRNZ0xURXdJQzAzTlRnZ0xUVTFORGdnTFRjMk9DQXROVFl6TXlBdE1pQXRNakFnTlNBdE1qQWdNemd3SUMweU15QnNNemd4Q2kweUlEWWdNemNnWXpNZ01qRWdNek1nTWprNUlEWTJJRFl4T0NBek15QXpNVGtnTmpNZ05Ua3dJRFkySURZd015QnNOQ0F5TWlBek5URWdNQ0JqTWpjeElEQWdNelV4SUMwekNqTTFOQ0F0TVRJZ01pQXROeUF6TXlBdE1qYzRJRGN3SUMwMk1ETWdNellnTFRNeU5DQTJPQ0F0TmpBM0lEY3hJQzAyTWpjZ2JEWWdMVE00SURNNE5DQXdJRE00TkNBd0lEQUtNalFnWXpBZ016SWdMVGN5T0NBMU5qRTNJQzAzTXpVZ05UWXpOQ0F0TXlBNUlDMHhNVFVnTVRJZ0xUVXhNQ0F4TWlBdE16azJJREFnTFRVd055QXRNeUF0TlRFd0lDMHhNbm9LYlRjMU5TQXRNell6TUNCak1DQXROU0F0TVRJeUlDMDRJQzB5TnpBZ0xUZ2diQzB5TnpFZ01DQTJJRE16SUdNeklERTNJRFl5SURVNE15QXhNekFnTVRJMU55QTJPU0EyTnpRS01USTVJREV5TXpZZ01UTXlJREV5TlRBZ05DQXhOeUF5TmpZZ0xUSTBNVGdnTWpjeklDMHlOVE15ZWlJdlBnbzhjR0YwYUNCa1BTSk5OamMzTlNBMk5qWTRJR010TXlBdE55QXROQ0F0TVRJNE1pQXRNeUF0TWpnek15QnNNeUF0TWpneU1DQXpPVGdnTFRNZ016azNJQzB5SURBZ01URTFNQW93SURFeE5UQWdOemtnTUNCak1USXdJREFnTXpJMElERTVJRFF6TVNBME1DQXpNak1nTmpJZ05UUXdJREU1TnlBMk9Ua2dORE0wSURFME1pQXlNVEFnTVRrd0lEUXdNU0F5TVRJS09EVXhJREU1SURNM01TQXpJRGd6TkNBdE16WWdNVEEyTlNBdE5qZ2dOREEwSUMwek1URWdOelUwSUMwMk1EVWdPRGMwSUMweU1EWWdPRFFnTFRRME1TQXhNRFlnTFRFeE5qTUtNVEEySUMwek1UY2dNQ0F0TkRBNUlDMHpJQzAwTVRJZ0xURXllaUJ0TVRFeE5pQXROell6SUdNek55QXROeUE0TkNBdE1qSWdNVEExSUMwek15QTVOQ0F0TkRrZ01UWTVDaTB4TmpjZ01UazRJQzB6TVRJZ05EY2dMVEl6TlNBME55QXRPVEV6SURBZ0xURXhOVEFnTFRnZ0xUUXhJQzB5T0NBdE1UQXpJQzAwTXlBdE1UTTJJQzAzTmlBdE1UWXpDaTB4T1RrZ0xUSXlNeUF0TkRVNElDMHlNalFnYkMweE1qTWdNQ0F3SURrME1TQXdJRGswTVNBeE1qZ2dMVGNnWXpjd0lDMHpJREUxTnlBdE1UTWdNVGt6SUMweU1Ib2lMejRLUEM5blBnbzhMM04yWno0SyINCiAgICAgIH0sDQogICAgICAicHJvamVjdEljb24iOiB7DQogICAgICAgICJsaWdodCI6IHsNCiAgICAgICAgICAiYnl0ZXMiOiAiaVZCT1J3MEtHZ29BQUFBTlNVaEVVZ0FBQUJnQUFBQVlDQVlBQUFEZ2R6MzRBQUFBQkdkQlRVRUFBTEdQQy94aEJRQUFBQWx3U0ZsekFBQU93Z0FBRHNJQkZTaEtnQUFBQVJwSlJFRlVTRXZ0bEdGeEFrRU1oVTlDSlZSQ0hSUUpsWUNFT2dBbjRBQWNGQWV0QkJ5QUE1cHY5NzVPZWdkM1BmaFZoamZ6aGlTYjVPMU5zalIzaGVmZ2EvQ3BlTlZcdTAwMkJxV1lCY1dJNUI1QmpQT2Yzc0F5ZWdyUGlWWHRYellMM0lMR2NBOGd4RGpmQnN4Z1Q4QndpSmhSWUI3OWFcdTAwMkJ5M1l3NWpBTnJnUEVpZFhLRUFkamJ2blB4Z1R3TFlaWWlJTHdLc0ZiSHdNZG9WdkZtQnJMQ1IyQ0lvc1lJL0pBdmxtTmhUNmtpOWs1WHNZRW5CNFx1MDAyQlpibUtjQUdzVWtYMzhLUWdHZXI0RWRyZHdYMEw4SW04XHUwMDJCTDlGdUJtXHUwMDJCSm5rZzhrQy9EcFVCWElUWC9Sa0FRdDU2b3ZXZHQvWkdvWUhHQ0JuNThRSHdhMTlxWkNHRnVXR0FQXHUwMDJCem1uOFhBSWd3QTc0bXJ4ckZlVHZ3YlVnY08vL0RQdkRBLzBIVGZBTkgyb3djWVhoMVFBQUFBQUJKUlU1RXJrSmdnZz09IiwNCiAgICAgICAgICAid2lkdGgiOiAyNCwNCiAgICAgICAgICAiaGVpZ2h0IjogMjQNCiAgICAgICAgfSwNCiAgICAgICAgImRhcmsiOiB7DQogICAgICAgICAgImJ5dGVzIjogImlWQk9SdzBLR2dvQUFBQU5TVWhFVWdBQUFCZ0FBQUFZQ0FZQUFBRGdkejM0QUFBQUJHZEJUVUVBQUxHUEMveGhCUUFBQUFsd1NGbHpBQUFPd2dBQURzSUJGU2hLZ0FBQUFCaEpSRUZVU0V2dHdRRUJBQUFBZ2lEL3I2NGhRQUFBWEEwSkdBQUIwQUpiWGdBQUFBQkpSVTVFcmtKZ2dnPT0iLA0KICAgICAgICAgICJ3aWR0aCI6IDI0LA0KICAgICAgICAgICJoZWlnaHQiOiAyNA0KICAgICAgICB9LA0KICAgICAgICAiaWNvRGF0YSI6ICJBQUFCQUFFQUdCZ0FBQUVBSUFDYkFRQUFGZ0FBQUlsUVRrY05DaG9LQUFBQURVbElSRklBQUFBWUFBQUFHQWdHQUFBQTRIYzlcdTAwMkJBQUFBQVJuUVUxQkFBQ3hqd3Y4WVFVQUFBQUpjRWhaY3dBQURzSUFBQTdDQVJVb1NvQUFBQUU5U1VSQlZFaEw3WlN4VFlNeEVJVi9ZQUhZZ0k0U09rcGdBemFBRFVnbUNLUEFCTEFCYkFCc0VEcEs2T2pJXHUwMDJCMncveTNHTVpKZVI4a2xQUGp2T25jOTN2NmV0NXlDTmNDeWRTai9TcjNRaEhVbGZFaHhLNXhMN3ZBZk9wQk9KOVhML0J2ZlNuM1FaWnRGXHUwMDJCaldaZ0pyRlc3Z0gyZUIwOVNabjlOUFpBQm9aVDF6eEtIOUoxVW1Ba0FFNC9vN2tXekR4STNBTGtBNHhtc0l4bU13UDRUbU5tSkFCRnh3RUZibVhRcERlQUhiNG4wVzB0eXVJSGVnUFVWOUxLNEVWYVNHUklQUUtqR2RDU2J0MzZ0SFFRbmNTNmF6V2N3WTFFTFZyd25keEtYR0dtRllBdnNzWnJPUERKTlx1MDAyQjY3eFg4QjZqdDJnQ3RwSHMwXHUwMDJCeWdEdVlUcmtMcHByYXhTUFx1MDAyQjM5bVFYUmxVTUtwS1k3ZkZKemJDZlB5WFdMXHUwMDJCRnMzOEZqVUQ3cVhSRUlSM2hKR1R1aHY0TXdGZFFEdkRPUTNBZm41enhqdDJiQS9UdEFJMnZqN1x1MDAyQnh1anlTd0FBQUFCSlJVNUVya0pnZ2c9PSINCiAgICAgIH0sDQogICAgICAiY2F0ZWdvcnlJY29uIjogew0KICAgICAgICAibGlnaHQiOiB7DQogICAgICAgICAgImJ5dGVzIjogImlWQk9SdzBLR2dvQUFBQU5TVWhFVWdBQUFCQUFBQUFRQ0FZQUFBQWY4LzloQUFBQUJHZEJUVUVBQUxHUEMveGhCUUFBQUFsd1NGbHpBQUFPd2dBQURzSUJGU2hLZ0FBQUFOOUpSRUZVT0UvdDBpR3pBVkVZaC9FTmdpZ0lvbkEvZ0NBSWduQ0REeUNJb3VCRENJSVBjYU1naUxvYkJVM1IzQ1l3SXdpQ3dQTzR6ZzZ6WjZTTi9qTy8yZldcdTAwMkJaOTg1dTBlU1Z5cG9vSWdXU2pEV2F2XHUwMDJCM2FVOWxDOC9wNFE5VlhPRWljNFoxRTNvbjdQQXlKRGJBWFRuQTN3V0UzaGNPNkNKTmJFQVRLMXpnUTZIbjFiVVx1MDAyQmt5WTJ3Tm9jMXR0NDdoM1JRWnJZZ0JGXHUwMDJCOElzXHUwMDJCUWsvdUxIem9lMklEcGxoZ2pURkNyNDVNWWdPVzJHQUxoNFdlMTB4OEg0OW5BQmY1MGZiNGZ0VGM4dHNCSHRNRTdtSUkvelNcdTAwMkJ1NHZkelF3ZXF6V3ZuXHUwMDJCU1hKTGtCWFhOQURBeC9BeUVBQUFBQVNVVk9SSzVDWUlJPSIsDQogICAgICAgICAgIndpZHRoIjogMTYsDQogICAgICAgICAgImhlaWdodCI6IDE2DQogICAgICAgIH0sDQogICAgICAgICJkYXJrIjogew0KICAgICAgICAgICJieXRlcyI6ICJpVkJPUncwS0dnb0FBQUFOU1VoRVVnQUFBQkFBQUFBUUNBWUFBQUFmOC85aEFBQUFCR2RCVFVFQUFMR1BDL3hoQlFBQUFBbHdTRmx6QUFBT3dnQUFEc0lCRlNoS2dBQUFBQk5KUkVGVU9FOWpHQVdqWUJTTUFqQmdZQUFBQkJBQUFhZEVmR01BQUFBQVNVVk9SSzVDWUlJPSIsDQogICAgICAgICAgIndpZHRoIjogMTYsDQogICAgICAgICAgImhlaWdodCI6IDE2DQogICAgICAgIH0NCiAgICAgIH0NCiAgICB9DQogIH0sDQogICJzZXR0aW5ncyI6IHsNCiAgICAiYnVpbGRQYXRoIjogImZpbGU6Ly8vQzovVXNlcnMvY2xhcmsuY2hlbmcvT25lRHJpdmUgLSBVQVAgQ29tcGFueS9EZXNrdG9wL19DT01NT05TL1VBUC1QbHVnaW5zLzAxX1JISU5PL2J1aWxkL3JoOCIsDQogICAgImJ1aWxkVGFyZ2V0Ijogew0KICAgICAgImhvc3QiOiB7DQogICAgICAgICJuYW1lIjogIlJoaW5vM0QiLA0KICAgICAgICAidmVyc2lvbiI6ICI4Ig0KICAgICAgfSwNCiAgICAgICJ0aXRsZSI6ICJSaGlubzNEICg4LiopIiwNCiAgICAgICJzbHVnIjogInJoOCINCiAgICB9LA0KICAgICJwdWJsaXNoVGFyZ2V0Ijogew0KICAgICAgInRpdGxlIjogIk1jTmVlbCBZYWsgU2VydmVyIg0KICAgIH0NCiAgfSwNCiAgImNvZGVzIjogW10NCn0=";
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
