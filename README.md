# Xamarin.Forms.HtmlLabel
Write a native formatted string using html string

[![NuGet](https://img.shields.io/nuget/v/Xamarin.Forms.HtmlLabel.svg)](https://www.nuget.org/packages/Xamarin.Forms.HtmlLabel/)
[![NuGet](https://img.shields.io/nuget/v/Xamarin.Forms.HtmlLabel.Firebase.svg)](https://www.nuget.org/packages/Xamarin.Forms.HtmlLabel.Firebase/)


## Getting started

- Install the Xamarin.Forms.HtmlLabel package

 ```
 Install-Package Xamarin.Forms.HtmlLabel -Version 0.0.1-pre
 ```

- Use the HtmlLabel on xaml

```xml
<HtmlLabel Text={Binding MyHtmltext}/>
```

- Bind to html text

```csharp
public class MyViewModel : BaseviewModel
{
	public string MyHtmltext => "<span>This is a <b>formmated text</b><br/>with <i>multilines<i></span>"
}
```