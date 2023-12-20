namespace ImageProcessingGUI

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Interactivity
open Avalonia.Platform.Storage
open ImageProcessing
open ImageProcessing.ImageProcessing
open ImageProcessing.Transformation
open ImageProcessing.RunStrategy
open Avalonia.Media.Imaging

type MainWindow() as this =
    inherit Window()

    let tmpPath = System.IO.Path.Combine([| __SOURCE_DIRECTORY__; "Assets"; ".temp" |])

    let mutable sourceImage = HelpProviders.Image([||], 0, 0, "")

    let mutable sourceImagePath = ""

    let mutable runStrategy = CPU

    let processFile (runStrategy: RunStrategy) transformation =
        let applicator =
            let ensuredRunStrategy = if GPUDevice.noGPU () then CPU else runStrategy

            match ensuredRunStrategy with
            | CPU -> getTsfCPU 1 transformation
            | _ -> getTsfGPU GPUDevice.context GPUDevice.localWorkSize transformation

        async { return applicator sourceImage }

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
                    sourceImage <- HelpProviders.loadAsImage filePath
                    sourceImagePath <- filePath

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
                    HelpProviders.saveImage sourceImage (System.IO.Path.Combine(dir, sourceImage.Name))
                with _ ->
                    ()
        }
        |> Async.StartImmediate

    member this.ResetChanges(sender: obj, args: RoutedEventArgs) =
        try
            sourceImage <- HelpProviders.loadAsImage sourceImagePath

            let imageControl = this.FindControl<Image>("Image")

            imageControl.Source <- new Bitmap(sourceImagePath)
        with exc ->
            ()

    member this.ApplyTransformation(sender: obj, args: RoutedEventArgs) =
        let transformation =
            match sender with
            | :? MenuItem as menuItem ->
                match menuItem.Name with
                | "Blur" -> Blur
                | "Edges" -> Edges
                | "Laplacian" -> Laplacian
                | "High-Pass" -> HighPass
                | "Vertical-Sobel" -> SobelV
                | "Clockwise" -> Rotate
                | "Counter-Clockwise" -> RotateCCW
                | "Horizontally" -> ReflectH
                | "Vertically" -> ReflectV
                | _ -> failwith ""
            | _ -> failwith ""

        let outPath = System.IO.Path.Combine(tmpPath, sourceImage.Name)

        task {
            let! newImage = processFile runStrategy transformation
            sourceImage <- newImage
            HelpProviders.saveImage sourceImage outPath

            this.FindControl<Image>("Image").Source <- new Bitmap(outPath)
        }
        |> Async.AwaitTask
        |> Async.StartImmediate

    member this.ChangeRunStrategy(sender: obj, args: RoutedEventArgs) =
        runStrategy <-
            match sender with
            | :? RadioButton as radioButton ->
                match radioButton.Name with
                | "CPU" -> CPU
                | "GPU" -> GPU
                | _ -> failwith ""
            | _ -> failwith ""

    member private this.InitializeComponent() =
#if DEBUG
        this.AttachDevTools()
#endif
        AvaloniaXamlLoader.Load(this)
