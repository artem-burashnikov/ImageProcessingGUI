namespace ImageProcessingGUI

open System
open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Interactivity
open Avalonia.Platform.Storage
open ImageProcessing
open ImageProcessing.ImageProcessing
open ImageProcessing.Transformation
open ImageProcessing.RunStrategy
open ImageProcessingGUI.Definitions
open Avalonia.Media.Imaging

type MainWindow() as this =
    inherit Window()

    let mutable globImage = HelpProviders.Image([||], 0, 0, "")

    let mutable globImagePath = ""

    let mutable runStrategy = CPU

    let processFile (runStrategy: RunStrategy) transformation =
        let applicator =
            let ensuredRunStrategy = if GPUDevice.noGPU () then CPU else runStrategy
            
            if ensuredRunStrategy = CPU then
                getTsfCPU 1 transformation
            else
                getTsfGPU GPUDevice.context GPUDevice.localWorkSize transformation

        async { return applicator globImage }

    do this.InitializeComponent()

    member this.LoadImage(sender: obj, args: RoutedEventArgs) =
        // Build a file picker Task
        let topLevel = TopLevel.GetTopLevel(this)

        let filePickerOpenOptions = FilePickerOpenOptions()
        filePickerOpenOptions.Title <- "Open image file..."
        filePickerOpenOptions.AllowMultiple <- false

        let openFileTask =
            topLevel.StorageProvider.OpenFilePickerAsync(filePickerOpenOptions)

        // Use separate thread to execute the Task
        async {
            let! fileSeq = Async.AwaitTask openFileTask

            if fileSeq.Count >= 1 then
                try
                    let filePath = fileSeq[0].TryGetLocalPath()

                    // Load imported image into memory for easy Reset.
                    globImage <- HelpProviders.loadAsImage filePath
                    globImagePath <- filePath

                    let imageControl = this.FindControl<Image>("Image")

                    imageControl.Source <- new Bitmap(filePath)
                with _ ->
                    ()
        }
        |> Async.StartImmediate

    member this.SaveImage(sender: obj, args: RoutedEventArgs) =
        // Build a directory picker Task
        let topLevel = TopLevel.GetTopLevel(this)

        let folderPickerOptions = FolderPickerOpenOptions()
        folderPickerOptions.Title <- "Save to..."

        let saveFileTask =
            topLevel.StorageProvider.OpenFolderPickerAsync(folderPickerOptions)

        // Use separate thread to execute the Task
        async {
            let! storage = Async.AwaitTask saveFileTask

            if storage.Count >= 1 then
                try
                    let dir = storage[0].TryGetLocalPath()
                    let name =
                        if System.IO.File.Exists(System.IO.Path.Combine(dir, globImage.Name)) then
                            String.concat "_" [DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"); globImage.Name]
                        else
                            globImage.Name
                    printfn $"%A{name}"
                    HelpProviders.saveImage globImage (System.IO.Path.Combine(dir, name))
                with _ ->
                    ()
        }
        |> Async.StartImmediate

    member this.ResetChanges(sender: obj, args: RoutedEventArgs) =
        try
            globImage <- HelpProviders.loadAsImage globImagePath

            let imageControl = this.FindControl<Image>("Image")

            imageControl.Source <- new Bitmap(globImagePath)
        with exc ->
            ()

    member this.ApplyTransformation(sender: obj, args: RoutedEventArgs) =
        let sender = sender :?> MenuItem
        
        let transformation =
            if sender.Name = "Blur" then
                Blur
            elif sender.Name = "Edges" then
                Edges
            elif sender.Name = "Laplacian" then
                Laplacian
            elif sender.Name = "High-Pass" then
                HighPass
            elif sender.Name = "Vertical-Sobel" then
                SobelV
            elif sender.Name = "Clockwise" then
                Rotate
            elif sender.Name = "Counter-Clockwise" then
                RotateCCW
            elif sender.Name = "Horizontally" then
                ReflectH
            else
                ReflectV

        let outPath = System.IO.Path.Combine(tmpPath, globImage.Name)

        task {
            let! newImage = processFile runStrategy transformation
            globImage <- newImage
            HelpProviders.saveImage globImage outPath
            this.FindControl<Image>("Image").Source <- new Bitmap(outPath)
        }
        |> Async.AwaitTask
        |> Async.StartImmediate

    member this.ChangeRunStrategy(sender: obj, args: RoutedEventArgs) =
        let sender = sender :?> RadioButton
        runStrategy <- if sender.Name = "GPU" then GPU else CPU

    member private this.InitializeComponent() =
#if DEBUG
        this.AttachDevTools()
#endif
        AvaloniaXamlLoader.Load(this)
