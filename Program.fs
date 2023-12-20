namespace ImageProcessingGUI

open System
open Avalonia

module Program =

    [<CompiledName "BuildAvaloniaApp">]
    let buildAvaloniaApp () =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace(areas = Array.empty)

    [<EntryPoint; STAThread>]
    let main argv =
        let tmpPath = System.IO.Path.Combine([| __SOURCE_DIRECTORY__; "Assets"; ".temp" |])
        System.IO.Directory.CreateDirectory(tmpPath) |> ignore

        try
            buildAvaloniaApp().StartWithClassicDesktopLifetime(argv)
        finally
            Array.iter System.IO.File.Delete (ImageProcessing.Streaming.listAllFiles (tmpPath))
            System.IO.Directory.Delete(tmpPath)
