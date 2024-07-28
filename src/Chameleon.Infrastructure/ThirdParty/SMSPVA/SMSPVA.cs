using Chameleon.Infrastructure.ThirdParty.SMSPVA.Models;
using System.Net.Http;
using System.Text.Json;

namespace Chameleon.Infrastructure.ThirdParty.SMSPVA;

public class SMSPVAService
{
    //make singleton claa
    private static SMSPVAService? _instance;
    public static SMSPVAService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SMSPVAService();
            }
            return _instance;
        }
    }
    private readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public string ApiKey { get; set; }
    public List<Models.Country> Countries { get; } =
    [
        new (1, "United States", "US"),
        new (2, "Canada", "CA"),
        new (3, "Unt. Kingdom", "UK"),
        new (4, "France", "FR"),
        new (5, "Germany", "DE"),
        new (6, "Italy", "IT"),
        new (7, "Spain", "ES"),
        new (8, "Albania", "AL"),
        new (9, "Argentina", "AR"),
        new (10, "Australia", "AU"),
        new (11, "Austria", "AT"),
        new (12, "Bangladesh", "BD"),
        new (13, "Bos. and Herz.", "BA"),
        new (14, "Brazil", "BR"),
        new (15, "Bulgaria", "BG"),
        new (16, "Cambodia", "KH"),
        new (17, "Chile", "CL"),
        new (18, "Colombia", "CO"),
        new (19, "Croatia", "HR"),
        new (20, "Cyprus", "CY"),
        new (21, "Czech Republic", "CZ"),
        new (22, "Denmark", "DK"),
        new (23, "Dominicana", "DO"),
        new (24, "Egypt", "EG"),
        new (25, "Estonia", "EE"),
        new (26, "Finland", "FI"),
        new (27, "Georgia", "GE"),
        new (28, "Ghana (Virtual)", "GH"),
        new (29, "Gibraltar", "GI"),
        new (30, "Greece", "GR"),
        new (31, "Hong Kong", "HK"),
        new (32, "Hungary", "HU"),
        new (33, "India", "IN"),
        new (34, "Japan", "JP"),
        new (35, "Kyrgyzstan (Virtual)", "KG"),
        new (36, "Malta", "MT"),
        new (37, "Norway", "NO"),
        new (38, "Pakistan (Virtual)", "PK"),
        new (39, "Singapore", "SG"),
        new (40, "Tanzania", "TZ"),
        new (41, "Uzbekistan (Virtual)", "UZ"),
        new (42, "Indonesia", "ID"),
        new (43, "Ireland", "IE"),
        new (44, "Israel", "IL"),
        new (45, "Kazakhstan", "KZ"),
        new (46, "Kenya", "KE"),
        new (47, "Laos", "LA"),
        new (48, "Latvia", "LV"),
        new (49, "Lithuania", "LT"),
        new (50, "Macedonia", "MK"),
        new (51, "Malaysia", "MY"),
        new (52, "Mexico", "MX"),
        new (53, "Morocco", "MA"),
        new (54, "Netherlands", "NL"),
        new (55, "New Zealand", "NZ"),
        new (56, "Nigeria", "NG"),
        new (57, "Paraguay", "PY"),
        new (58, "Philippines", "PH"),
        new (59, "Poland", "PL"),
        new (60, "Portugal", "PT"),
        new (61, "Romania", "RO"),
        new (62, "Russian Federation", "RU"),
        new (63, "Serbia", "RS"),
        new (64, "Slovakia", "SK"),
        new (65, "Slovenia", "SI"),
        new (66, "South Africa", "ZA"),
        new (67, "Sweden", "SE"),
        new (68, "Thailand", "TH"),
        new (69, "Turkey", "TR"),
        new (70, "Ukraine", "UA"),
        new (71, "Vietnam", "VN")
    ];
    public List<Models.Service> Services { get; } =
    [
        new(1, "", "OpenAI API (chatGPT, DALL-e 2)", "opt132"),
        new(2, "", "22bet", "opt224"),
        new(3, "", "888casino", "opt22"),
        new(4, "", "Abbott", "opt242"),
        new(5, "", "Adidas & Nike", "opt86"),
        new(6, "", "Airbnb", "opt46"),
        new(7, "", "Alibaba (Taobao, 1688.com)", "opt61"),
        new(8, "", "Amazon", "opt44"),
        new(9, "", "AOL", "opt10"),
        new(10, "", "Apple", "opt131"),
        new(11, "", "autocosmos.com", "opt143"),
        new(12, "", "Avito", "opt59"),
        new(13, "", "Badoo", "opt56"),
        new(14, "", "BANDUS", "opt209"),
        new(15, "", "Bazos.sk", "opt138"),
        new(16, "", "Beget.com", "opt187"),
        new(17, "", "bet365", "opt17"),
        new(18, "", "Betano (+BETANO.ro)", "opt192"),
        new(19, "", "BetFair", "opt25"),
        new(20, "", "Betmgm", "opt223"),
        new(21, "", "Bitpanda", "opt237"),
        new(22, "", "Blizzard", "opt78"),
        new(23, "", "blsspain-russia.com", "opt135"),
        new(24, "", "Bolt", "opt81"),
        new(25, "", "Brevo", "opt217"),
        new(26, "", "bumble", "opt145"),
        new(27, "", "bunq", "opt199"),
        new(28, "", "bwin", "opt137"),
        new(29, "", "Careem", "opt89"),
        new(30, "", "casa.it", "opt148"),
        new(31, "", "Cash App", "opt226"),
        new(32, "", "Cashrewards", "opt214"),
        new(33, "", "Casino Plus", "opt201"),
        new(34, "", "ChoTot", "opt176"),
        new(35, "", "CityMobil", "opt76"),
        new(36, "", "Claude (Anthropic)", "opt196"),
        new(37, "", "Clubhouse", "opt98"),
        new(38, "", "CoinBase", "opt112"),
        new(39, "", "CONTACT", "opt51"),
        new(40, "", "Craigslist", "opt26"),
        new(41, "", "Credit Karma", "opt124"),
        new(42, "", "CupidMedia", "opt157"),
        new(43, "", "Czech email services", "opt150"),
        new(44, "", "Deliveroo", "opt53"),
        new(45, "", "DenimApp", "opt204"),
        new(46, "", "DiDi", "opt92"),
        new(47, "", "Discord", "opt45"),
        new(48, "", "DistroKid", "opt232"),
        new(49, "", "Dodopizza + PapaJohns", "opt27"),
        new(50, "", "Doordash", "opt40"),
        new(51, "", "Drom.RU", "opt32"),
        new(52, "", "Drug Vokrug", "opt31"),
        new(53, "", "dundle", "opt136"),
        new(54, "", "EasyPay", "opt21"),
        new(55, "", "ENEBA", "opt200"),
        new(56, "", "EUROBET", "opt141"),
        new(57, "", "Facebook", "opt2"),
        new(58, "", "FastMail", "opt43"),
        new(59, "", "Fbet", "opt215"),
        new(60, "", "Feeld", "opt159"),
        new(61, "", "Fiverr", "opt6"),
        new(62, "", "fontbet", "opt139"),
        new(63, "", "foodora", "opt189"),
        new(64, "", "foodpanda", "opt115"),
        new(65, "", "Fortuna", "opt221"),
        new(66, "", "Fotostrana", "opt13"),
        new(67, "", "funpay", "opt142"),
        new(68, "", "G2A.COM", "opt68"),
        new(69, "", "Gameflip", "opt77"),
        new(70, "", "Gamers set (offgamers.com, G2A.com, seagm.com)", "opt28"),
        new(71, "", "GetsBet.ro", "opt179"),
        new(72, "", "GetTaxi", "opt35"),
        new(73, "", "GGbet", "opt188"),
        new(74, "", "GGPokerUK", "opt229"),
        new(75, "", "giocodigitale.it", "opt85"),
        new(76, "", "Glovo & Raketa", "opt108"),
        new(77, "", "goldbet.it", "opt240"),
        new(78, "", "Google (YouTube, Gmail)", "opt1"),
        new(79, "", "Google Voice", "opt140"),
        new(80, "", "GrabTaxi", "opt30"),
        new(81, "", "Grailed", "opt420"),
        new(82, "", "Grindr", "opt110"),
        new(83, "", "Happn", "opt155"),
        new(84, "", "HelloTalk", "opt203"),
        new(85, "", "hepsiburada", "opt238"),
        new(86, "", "Hey", "opt216"),
        new(87, "", "Hinge", "opt120"),
        new(88, "", "hopper", "opt144"),
        new(89, "", "HUAWEI", "opt166"),
        new(90, "", "ICard", "opt103"),
        new(91, "", "idealista.com", "opt165"),
        new(92, "", "ifood", "opt55"),
        new(93, "", "IMO", "opt111"),
        new(94, "", "inbox.lv", "opt167"),
        new(95, "", "Inboxdollars", "opt118"),
        new(96, "", "Instagram (+Threads)", "opt16"),
        new(97, "", "Ipsos", "opt193"),
        new(98, "", "IQOS", "opt243"),
        new(99, "", "JD.com", "opt94"),
        new(100, "", "KakaoTalk", "opt71"),
        new(101, "", "Klarna", "opt175"),
        new(102, "", "kleinanzeigen.de", "opt152"),
        new(103, "", "KoronaPay", "opt99"),
        new(104, "", "Kuper (SberMarket)", "opt97"),
        new(105, "", "kwiff.com", "opt129"),
        new(106, "", "Lajumate.ro", "opt195"),
        new(107, "", "Lalamove", "opt180"),
        new(108, "", "LAPOSTE", "opt182"),
        new(109, "", "LASVEGAS.RO", "opt222"),
        new(110, "", "Lazada", "opt60"),
        new(111, "", "Leboncoin", "opt164"),
        new(112, "", "Line Messenger", "opt37"),
        new(113, "", "LinkedIn", "opt8"),
        new(114, "", "LiveScore", "opt42"),
        new(115, "", "LocalBitcoins", "opt105"),
        new(116, "", "Locanto.com", "opt114"),
        new(117, "", "Lyft", "opt75"),
        new(118, "", "Magnit", "opt126"),
        new(119, "", "Mail.RU", "opt33"),
        new(120, "", "Mail.ru Group", "opt4"),
        new(121, "", "Mamba", "opt100"),
        new(122, "", "Marktplaats", "opt171"),
        new(123, "", "maxline.by", "opt219"),
        new(124, "", "MiChat", "opt96"),
        new(125, "", "Microsoft (Azure, Bing, Skype, etc)", "opt15"),
        new(126, "", "mobileDE", "opt156"),
        new(127, "", "MOMO", "opt184"),
        new(128, "", "Monese", "opt121"),
        new(129, "", "MoneyLion", "opt47"),
        new(130, "", "MPSellers", "opt197"),
        new(131, "", "MrGreen", "opt211"),
        new(132, "", "MS Office 365", "opt7"),
        new(133, "", "myopinions & erewards", "opt0"),
        new(134, "", "Naver", "opt73"),
        new(135, "", "Nectar", "opt198"),
        new(136, "", "NetBet", "opt95"),
        new(137, "", "Neteller", "opt116"),
        new(138, "", "Netflix", "opt101"),
        new(139, "", "NHNCloud", "opt202"),
        new(140, "", "NHNcorp (강남언니)", "opt177"),
        new(141, "", "Nico", "opt119"),
        new(142, "", "novibet.com", "opt151"),
        new(143, "", "OD", "opt5"),
        new(144, "", "OfferUp", "opt113"),
        new(145, "", "OkCupid", "opt230"),
        new(146, "", "OKX", "opt228"),
        new(147, "", "OLX + goods.ru", "opt70"),
        new(148, "", "onet.pl (Onet Konto)", "opt241"),
        new(149, "", "OTHER (no guarantee)", "opt19"),
        new(150, "", "OTHER (voice code)", "opt00019"),
        new(151, "", "OurTime", "opt212"),
        new(152, "", "OZON.ru", "opt181"),
        new(153, "", "Paddy Power", "opt109"),
        new(154, "", "Pari.ru", "opt169"),
        new(155, "", "Parimatch", "opt3"),
        new(156, "", "Payoneer", "opt162"),
        new(157, "", "PayPal + Ebay", "opt83"),
        new(158, "", "Paysafecard", "opt122"),
        new(159, "", "PAYSEND", "opt183"),
        new(160, "", "pm.by", "opt149"),
        new(161, "", "POF.com", "opt84"),
        new(162, "", "Prom.UA", "opt107"),
        new(163, "", "Proton Mail", "opt57"),
        new(164, "", "Publi24", "opt207"),
        new(165, "", "Qiwi", "opt18"),
        new(166, "", "Rambler.ru", "opt154"),
        new(167, "", "Revolut", "opt133"),
        new(168, "", "ROOMSTER", "opt153"),
        new(169, "", "Royal Canin", "opt170"),
        new(170, "", "RusDate", "opt186"),
        new(171, "", "Samokat", "opt185"),
        new(172, "", "Samsung", "opt174"),
        new(173, "", "Schibsted-konto", "opt134"),
        new(174, "", "Shopee", "opt48"),
        new(175, "", "Signal", "opt127"),
        new(176, "", "Sisal", "opt38"),
        new(177, "", "Skout", "opt49"),
        new(178, "", "Skrill", "opt117"),
        new(179, "", "Snapchat", "opt90"),
        new(180, "", "SNKRDUNK", "opt190"),
        new(181, "", "Solitaire Cash", "opt234"),
        new(182, "", "Steam", "opt58"),
        new(183, "", "subito.it", "opt146"),
        new(184, "", "Swagbucks", "opt125"),
        new(185, "", "Tango", "opt82"),
        new(186, "", "TANK.RU", "opt161"),
        new(187, "", "Taptap", "opt239"),
        new(188, "", "Taxi Maksim", "opt74"),
        new(189, "", "Telegram", "opt29"),
        new(190, "", "Telegram (voice code)", "opt00029"),
        new(191, "", "Tencent QQ", "opt34"),
        new(192, "", "Ticketmaster", "opt52"),
        new(193, "", "TikTok", "opt104"),
        new(194, "", "Tinder", "opt9"),
        new(195, "", "TLScontact", "opt235"),
        new(196, "", "TopCashback", "opt191"),
        new(197, "", "TOTOGAMING", "opt220"),
        new(198, "", "TransferGo", "opt218"),
        new(199, "", "TrueCaller", "opt233"),
        new(200, "", "Truth Social", "opt244"),
        new(201, "", "Twilio", "opt66"),
        new(202, "", "Twitch", "opt205"),
        new(203, "", "U By Prodia", "opt160"),
        new(204, "", "Uber", "opt72"),
        new(205, "", "Verse", "opt39"),
        new(206, "", "Viber", "opt11"),
        new(207, "", "Vinted", "opt130"),
        new(208, "", "VK", "opt69"),
        new(209, "", "VonageVF", "opt178"),
        new(210, "", "VooV Meeting", "opt147"),
        new(211, "", "Waitomo", "opt213"),
        new(212, "", "WalletHub", "opt206"),
        new(213, "", "Walmart", "opt227"),
        new(214, "", "WEB.DE", "opt172"),
        new(215, "", "WebMoney&ENUM", "opt24"),
        new(216, "", "WeChat", "opt67"),
        new(217, "", "Weebly", "opt54"),
        new(218, "", "WESTSTEIN", "opt80"),
        new(219, "", "Whatnot", "opt231"),
        new(220, "", "WhatsAPP", "opt20"),
        new(221, "", "WhatsAPP (voice code)", "opt00020"),
        new(222, "", "Whoosh", "opt123"),
        new(223, "", "Wing Money", "opt106"),
        new(224, "", "Wise", "opt91"),
        new(225, "", "Wolt", "opt163"),
        new(226, "", "WooPlus", "opt208"),
        new(227, "", "X (Twitter)", "opt41"),
        new(228, "", "X World Wallet", "opt173"),
        new(229, "", "Yahoo", "opt65"),
        new(230, "", "Yalla.live", "opt88"),
        new(231, "", "Yandex&YooMoney", "opt23"),
        new(232, "", "Year13", "opt236"),
        new(233, "", "Zalo", "opt158"),
        new(234, "", "Zasilkovna", "opt225"),
        new(235, "", "Zoho", "opt93"),
        new(236, "", "ZoomInfo", "opt194") 
    ];


    public async Task<ApiResponse<T>> GetActivationNumberAsync<T>(Models.Country country, Models.Service service)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("apikey", ApiKey);

        HttpResponseMessage response = await client.GetAsync($"https://api.smspva.com/activation/number/{country.Code}/{service.Code}");
        string responseBody = await response.Content.ReadAsStringAsync();

        var jsonResponse = JsonSerializer.Deserialize<ApiResponse<T>>(responseBody, options);
        return jsonResponse;
    }

    public async Task<ApiResponse<T>> ReceiveSMS<T>(int id)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("apikey", ApiKey);
        //client.DefaultRequestHeaders.Add("partnerkey", "SOME_STRING_VALUE");

        HttpResponseMessage response = await client.GetAsync($"https://api.smspva.com/activation/sms/{id}");
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        var jsonResponse = JsonSerializer.Deserialize<ApiResponse<T>>(responseBody, options);
        return jsonResponse;
    }

    public async Task<string> GetBalanceAsync()
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("apikey", ApiKey);

        HttpResponseMessage response = await client.GetAsync("https://api.smspva.com/activation/balance");
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}

