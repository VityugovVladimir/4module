using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;

namespace AvaloniaApplication1;

public partial class MainWindow : Window
{
    string DataFromApi = "";
    public MainWindow()
    {
        InitializeComponent();
    }

    private void SendTestResult_OnClick(object? sender, RoutedEventArgs e)
    {
        var pattern = @"^\d{10}(\d{2})?$";
        var validationResult = Regex.IsMatch(DataFromApi, pattern);
        ValidationResultTBlock.Text = validationResult ? "ИНН не содержит запрещенные символы" : "ИНН содержит запрещенные символы";
        
        using var doc = WordprocessingDocument.Open(@"TestCase.docx", true);
        var document = doc.MainDocumentPart.Document;

        if (document.Descendants<Text>().FirstOrDefault(text => text.Text.Contains("Result 1")) != null)
        {
            ReplaceTextTestCase("Result 1", validationResult, document);
        } else if (document.Descendants<Text>().FirstOrDefault(text => text.Text.Contains("Result 2")) != null)
        {
            ReplaceTextTestCase("Result 2", validationResult, document);
        }
        
    }

   

    private void ReplaceTextTestCase(string replacedText, bool validationResult, Document document)
    {
        foreach (var text in document.Descendants<Text>())
        {
            if (text.Text == replacedText)
                text.Text = text.Text.Replace(replacedText, validationResult ? "Успешно" : "Не успешно");
            else if (text.Text == replacedText) 
                text.Text = text.Text.Replace(replacedText, validationResult ? "Успешно" : "Не успешно");
        }
    }


    private async void GetDataFromApi_OnClick(object? sender, RoutedEventArgs e)
    {
        var httpClient = new HttpClient();
        var content = await httpClient.GetStringAsync("http://localhost:4444/TransferSimulator/inn");
        var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        DataFromApi = data["value"];
        DataFromApiTBlock.Text = DataFromApi;
    }
}
