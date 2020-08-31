using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class GlobalReferences
{
    public static string _WPRestAPIEndpoint = "https://www.huperoptikusa.com/wp-json/wcra/v1/";

    public static string _DealerStoreLink = "https://www.huperoptikusa.com/shop/";
    public static string _DocumentsLink = "https://www.huperoptikusa.com/downloads/";
    public static string _HuperNewsLink = "https://www.huperoptikusa.com/news/";
    public static string _GoogleDriveLink = "https://drive.google.com/drive/folders/16gGUJ8HemxSAh4ewl8cYLo0y6G8svuRk";
    public static string _WarrantlyLink = "https://formstackpro.formstack.com/forms/warranty_registration_final";
    public static string _PhotoGalleryLink = "https://www.huperoptikusa.com/rl_gallery/huper-optik-gallery/";
    public static string _MarketingLink = "https://www.huperoptikusa.com/marketing/";
    public static string _HuperOptikDealer = "https://formstackpro.formstack.com/forms/become_a_huper_optik_dealer";

    public static bool _IsPhotoLoaded = false;
    public static int _SelectedZoomImage = -1;

    public static int _PointIndex = 0;
    public static List<Transform> _Points = new List<Transform>();
    public static int _WindowIndex = 0;
    public static List<GameObject> _Windows = new List<GameObject>();
    public static string _Material = "";
    public static FilmType _MaterialType = FilmType.None;
    public static FilmType _LastFilmSeries = FilmType.None;
    public static bool _IsOutside = false;

    public static string[] _MeasurementUnitNamesAbbr = { "m", "cm", "mm", "yd", "ft", "in" };
    public static string[] _MeasurementUnitNames = { "meter", "centimeter", "millimeter", "yard", "foot", "inch" };
    public static float[] _MeasurementFactors = { 1, 100, 1000, 1.09361f, 3.28084f, 39.3701f };

    public static float[] _FilmTypeRegularCoreInLbs = {
        0.018654f, 
        0.027006f,
        0.026578f,
        0.026798f,
        0.026714f,
        0.019934f,
        0.02671f,
        0.01993f,
        0.019154f,
        0.01907f,
        0.039614f,
        0.07251f,
        0.049438f,
        0.032274f,
        0.018338f,
        0.022882f,
        0.018562f,
        0.019714f,
        0.030462f,
        0.02684f,
        0.021872f,
        0.0248f,
        0.021522f
    };

    public static float[] _FilmTypeRegularCoreInKg = {
        0.008461454f,
        0.012249922f,
        0.012055781f,
        0.012155573f,
        0.01211747f,
        0.009042062f,
        0.012115656f,
        0.009040248f,
        0.008688254f,
        0.008650152f,
        0.01796891f,
        0.032890536f,
        0.022425077f,
        0.014639486f,
        0.008318117f,
        0.010379275f,
        0.008419723f,
        0.00894227f,
        0.013817563f,
        0.012174624f,
        0.009921139f,
        0.01124928f,
        0.009762379f
    };

    public static float[] _CoreLengthInLbs = {
        0.38f,
        0.63f,
        0.77f,
        0.95f,
        0.975f,
        1.04f,
        1.39f,
        1.588f,
        2.22f,
        1.5f,
        2.03f,
        2.62f,
        3f
    };

    public static float[] _CoreLengthInKg = {
        0.172368f,
        0.285768f,
        0.349272f,
        0.43092f,
        0.44226f,
        0.471744f,
        0.630504f,
        0.7203168f,
        1.006992f,
        0.6804f,
        0.920808f,
        1.188432f,
        1.3608f
    };
}

public enum BuildSceneNames
{
    Splash = 0,  
    Main = 1, 
    FilmVisualizer = 2, 
    Measure = 3
}

[Serializable]
public enum FilmType
{
    None = 0x0,
    Fusion = 0x0C77BC,
    Ceramic = 0x82BE41,
    Select = 0xF28A21,
    Dekorativ = 0x6CC8C6
}

[Serializable]
public enum MeasurementUnit
{
    METER = 0,
    CENTIMETER = 1,
    MILLIMETER = 2,
    YARD = 3, 
    FOOT = 4, 
    INCH = 5, 
}

[Serializable]
public enum FilmTypeRegularCore
{
    C05 = 0, 
    C15 = 1, 
    C20 = 2, 
    C30 = 3, 
    C40 = 4, 
    C45 = 5, 
    C50 = 6, 
    C60 = 7, 
    CS_4_MIL = 8, 
    CS_8_MIL = 9, 
    Dusted_Crystal = 10, 
    Frost = 11, 
    Fusion = 12, 
    Matte_Black = 13, 
    Silver = 14, 
    THERM_X = 15, 
    Whiteout = 16, 
    C70 = 17, 
    Drei = 18, 
    KLAR_85 = 19, 
    Sech = 20
}

[Serializable]
public enum CoreLength
{
    _12Inch = 0, 
    _20Inch = 1, 
    _24Inch = 2,
    _30Inch = 3,
    _36Inch = 4,
    _40Inch = 5,
    _48Inch = 6,
    _60Inch = 7,
    _72Inch = 8,
    _36Inch_Thick = 9,
    _48Inch_Thick = 10,
    _60Inch_Thick = 11,
    _72Inch_Thick = 12,
}

[Serializable]
public class RESTAPI_Response
{
    public string status;
    public string response;
    public int code;
    public string data;

    public RESTAPI_Response()
    {
        status = response = "";
        code = 500;
        data = null;
    }
}