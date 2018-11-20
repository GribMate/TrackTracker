using System;



namespace TrackTracker.BLL.Enums
{
    /*
     * Supported lyrics languages of a track.
     * These might be mixed in international songs or collaborations.
    */
    [Flags]
    public enum LyricsLanguages : long
    {
        ALB = 1 << 0,  // Albanian
        ARA = 1 << 1,  // Arabic
        BOS = 1 << 2,  // Bosnian
        BUL = 1 << 3,  // Bulgarian
        CAT = 1 << 4,  // Catalan
        CHI = 1 << 5,  // Chinese
        CZE = 1 << 6,  // Czech
        DAN = 1 << 7,  // Danish
        DUT = 1 << 8,  // Dutch
        ENG = 1 << 9,  // English
        EPO = 1 << 10,  // Esperanto
        EST = 1 << 11,  // Estonian
        FIN = 1 << 12,  // Finnish
        FRE = 1 << 13,  // French
        GER = 1 << 14,  // German
        GLE = 1 << 15,  // Irish
        GRE = 1 << 16,  // Greek
        HEB = 1 << 17,  // Hebrew
        HIN = 1 << 18,  // Hindi
        HRV = 1 << 19,  // Croatian
        HUN = 1 << 20,  // Hungarian
        ICE = 1 << 21,  // Icelandic
        IND = 1 << 22,  // Indonesian
        ITA = 1 << 23,  // Italian
        JPN = 1 << 24,  // Japanese
        KAZ = 1 << 25,  // Kazakh
        KOR = 1 << 26,  // Korean
        LAT = 1 << 27,  // Latin
        LIT = 1 << 28,  // Lithuanian
        LTZ = 1 << 29,  // Luxembourgish
        MAY = 1 << 30,  // Malay
        NAV = 1 << 31,  // Navajo
        NEP = 1 << 32,  // Nepali
        NOR = 1 << 33,  // Norwegian
        PAN = 1 << 34,  // Punjabi
        PER = 1 << 35,  // Persian
        POL = 1 << 36,  // Polish
        POR = 1 << 37,  // Portuguese
        RUM = 1 << 38,  // Romanian
        RUS = 1 << 39,  // Russian
        SLO = 1 << 40,  // Slovak
        SLV = 1 << 41,  // Slovenian
        SPA = 1 << 42,  // Spanish
        SRP = 1 << 43,  // Serbian
        SWE = 1 << 44,  // Swedish
        THA = 1 << 45,  // Thai
        TIB = 1 << 46,  // Tibetan
        TON = 1 << 47,  // Tonga
        TUR = 1 << 48,  // Turkish
        UKR = 1 << 49,  // Ukrainian
        UZB = 1 << 50,  // Uzbek
        VIE = 1 << 51,  // Vietnamese
        WEL = 1 << 52,  // Welsh
        ZUL = 1 << 53,  // Zulu

        Unknown = 0 // Default
    }
}
