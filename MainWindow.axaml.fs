namespace ImageProcessingGUI

open System.Threading.Tasks
open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Interactivity
open Avalonia.Platform.Storage
open Avalonia.Styling
open System.IO
open Avalonia.Media.Imaging
open SkiaSharp

type MainWindow () as this = 
    inherit Window ()

    do this.InitializeComponent()
        
    member this.LoadImage(sender: obj, args: RoutedEventArgs) =
        let topLevel = TopLevel.GetTopLevel(this)
        let filePickerOpenOptions = FilePickerOpenOptions()
        filePickerOpenOptions.Title <- "Choose an image file"
        filePickerOpenOptions.AllowMultiple <- false
        
        let openFileTask = topLevel.StorageProvider.OpenFilePickerAsync(filePickerOpenOptions)
        
        async {
            let! fileSeq = Async.AwaitTask openFileTask
            if fileSeq.Count >= 1 then
                try
                    let filePath = fileSeq[0].TryGetLocalPath()
                    let imageControl = this.FindControl<Image>("Image")
                    imageControl.Source <- new Bitmap(filePath)
                with _ -> ()
        }
        |> Async.StartImmediate
            
    member this.SaveImage(sender: obj, args: RoutedEventArgs) =
        ()
    
    member this.ChangeProcessingUnit(sender: obj, args: RoutedEventArgs) =
        ()
    
    member this.ResetChanges(sender: obj, args: RoutedEventArgs) =
        ()    
    
    member this.FilterImage(sender: obj, args: RoutedEventArgs) =
        ()
    member this.RotateImage(sender: obj, args: RoutedEventArgs) =
        ()  
    member this.ReflectImage(sender: obj, args: RoutedEventArgs) =
        ()

    // member this.ChangeOpenBorderVisibility(source: obj, args: Avalonia.Input.TappedEventArgs) =
       
    member private this.InitializeComponent() =
        #if DEBUG
            this.AttachDevTools()
        #endif
            AvaloniaXamlLoader.Load(this)
