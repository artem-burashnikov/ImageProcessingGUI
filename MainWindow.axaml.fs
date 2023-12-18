namespace ImageProcessingGUI

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Interactivity
open Avalonia.Styling
open System.IO
open Avalonia.Media.Imaging

type MainWindow () as this = 
    inherit Window ()

    do this.InitializeComponent()
    
    member this.ChangeProcessingUnit(sender: obj, args: RoutedEventArgs) =
        ()
    
    member this.ResetChanges(sender: obj, args: RoutedEventArgs) =
        ()
    
    member this.OpenButtonClick(sender: obj, args: RoutedEventArgs) =  
        ()

        let inputCtrl = this.FindControl<Border>("InputPath")
        let outputCtrl = this.FindControl<Border>("OutputPath")

        inputCtrl.IsVisible <- true
        outputCtrl.IsVisible <- false
    
    member this.SaveButtonClick(sender: obj, args: RoutedEventArgs) =
        let inputCtrl = this.FindControl<Border>("InputPath")
        let outputCtrl = this.FindControl<Border>("OutputPath")

        inputCtrl.IsVisible <- false
        outputCtrl.IsVisible <- true
        
    member this.LoadImage(sender: obj, args: RoutedEventArgs) =
        ()
    member this.SaveImage(sender: obj, args: RoutedEventArgs) =
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
