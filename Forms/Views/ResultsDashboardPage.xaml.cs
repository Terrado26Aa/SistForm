using Forms.Models;
using Forms.Services;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Forms.Views;

public partial class ResultsDashboardPage : ContentPage
{
    private readonly int _formId;
    private readonly string _formTitle;
    private List<SubmitResponseDto> _allResults = new();

    public ResultsDashboardPage(int formId, string formTitle)
    {
        InitializeComponent();
        _formId = formId;
        _formTitle = formTitle;
        FormTitleLabel.Text = $"Resultados: {_formTitle}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadResults();
    }

    private async Task LoadResults()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        var api = new ApiService();
        var result = await api.GetFormResultsAsync(_formId);

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;

        if (result.IsSuccess)
        {
            _allResults = result.Value;
            ResultsList.ItemsSource = _allResults;
            ShowAllOnMap();
        }
        else
        {
            await DisplayAlert("Error", result.ErrorMessage, "OK");
        }
    }

    private void ShowAllOnMap()
    {
        ResultsMap.Pins.Clear();
        ResultsMap.MapElements.Clear();

        if (_allResults.Count == 0) return;

        Location? firstLocation = null;

        foreach (var res in _allResults)
        {
            if (res.LatitudeA.HasValue && res.LongitudeA.HasValue)
            {
                var loc = new Location(res.LatitudeA.Value, res.LongitudeA.Value);
                firstLocation ??= loc;

                ResultsMap.Pins.Add(new Pin
                {
                    Label = $"Inicio - {res.SaveAt:HH:mm}",
                    Location = loc,
                    Type = PinType.Place
                });
            }

            if (res.LatitudeB.HasValue && res.LongitudeB.HasValue)
            {
                ResultsMap.Pins.Add(new Pin
                {
                    Label = $"Fin - {res.SaveAt:HH:mm}",
                    Location = new Location(res.LatitudeB.Value, res.LongitudeB.Value),
                    Type = PinType.Generic
                });
            }
        }

        if (firstLocation != null)
        {
            ResultsMap.MoveToRegion(MapSpan.FromCenterAndRadius(firstLocation, Distance.FromKilometers(1)));
        }
    }

    private void OnResultSelected(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection.FirstOrDefault() as SubmitResponseDto;
        if (selected == null) return;

        ResultsMap.Pins.Clear();
        ResultsMap.MapElements.Clear();

        if (selected.LatitudeA.HasValue && selected.LongitudeA.HasValue)
        {
            var locA = new Location(selected.LatitudeA.Value, selected.LongitudeA.Value);
            ResultsMap.Pins.Add(new Pin { Label = "Punto A", Location = locA, Type = PinType.Place });
            ResultsMap.MoveToRegion(MapSpan.FromCenterAndRadius(locA, Distance.FromMeters(500)));
        }

        if (selected.LatitudeB.HasValue && selected.LongitudeB.HasValue)
        {
            var locB = new Location(selected.LatitudeB.Value, selected.LongitudeB.Value);
            ResultsMap.Pins.Add(new Pin { Label = "Punto B", Location = locB, Type = PinType.SearchResult });
        }

        // Dibujar ruta (GeoJSON)
        if (!string.IsNullOrEmpty(selected.RoutePath))
        {
            try
            {
                // Simple parsing del GeoJSON (esperamos un Feature con LineString)
                dynamic geoData = JsonConvert.DeserializeObject(selected.RoutePath);
                var coordinates = geoData.geometry.coordinates;

                var polyline = new Polyline
                {
                    StrokeColor = Colors.Blue,
                    StrokeWidth = 5
                };

                foreach (var coord in coordinates)
                {
                    double lon = coord[0];
                    double lat = coord[1];
                    polyline.Geopath.Add(new Location(lat, lon));
                }

                ResultsMap.MapElements.Add(polyline);
            }
            catch { }
        }
    }
}
