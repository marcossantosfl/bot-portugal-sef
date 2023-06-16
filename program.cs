using CsvHelper;
using GrabberProxy.model;
using GrabberProxy.thread;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using HtmlAgilityPack;

#pragma warning disable CS0168


internal class Program
{
    static string access_token = "token";
    static bestcaptchasolver.BestCaptchaSolverAPI bcs = new bestcaptchasolver.BestCaptchaSolverAPI(access_token);

    static int process = 0;

    static DateTime date1 = DateTime.Now;
    static DateTime date2 = DateTime.Now;

    static async Task Main(string[] args)
    {
       
    }


    private static async Task selectProcess()
    {
        try
        {
            var clientHandler = new HttpClientHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            var client = new HttpClient(clientHandler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://sapa.sef.pt/Agendamento/SeleccionarProcesso"),
                Headers =
    {
        { "host", "sapa.sef.pt" },
        { "connection", "keep-alive" },
        { "cache-control", "max-age=0" },
        { "sec-ch-ua", "\"Not_A Brand\";v=\"99\", \"Google Chrome\";v=\"109\", \"Chromium\";v=\"109\"" },
        { "sec-ch-ua-mobile", "?0" },
        { "sec-ch-ua-platform", "\"Windows\"" },
        { "upgrade-insecure-requests", "1" },
        { "user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36" },
        { "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
        { "sec-fetch-site", "same-origin" },
        { "sec-fetch-mode", "navigate" },
        { "sec-fetch-user", "?1" },
        { "sec-fetch-dest", "document" },
        { "referer", "https://sapa.sef.pt/Account/Login?ReturnUrl=%2FAgendamento%2FSeleccionarProcesso" },
        { "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" },
        { "cookie", "__AntiXsrfToken=c5b08f3e174949a69befc634cfa72c34; .ASPXAUTH=BFB10506693421C8015FF4CC5B9CA39D585577B8C03D143B2EFB974E3BB1D163FE4CA8F50DAFE8C72817C5775C005D474308F40C28C70BAD70518E9289B13E80A5CE301F1AC69524BA3717FBFE7A7C1D3CC8D36FEB130ECBCA9C24DA26667A4DB8666A78966EC3E28F05F6E2C395782EF9D3A89FE330B1AD2EB820180FD6E6B2; .AspNet.ApplicationCookie=VaGi1ISyK5ClnmKXxTnnW9efgnjhMsR14tLRW7DvGCAsFFcXsrltc2yVrjvMeyJllX94T4pjzlVsJNM1_A09LTeQG3hYLhHqkppRBI3M3ObG5whKLgEeD27x8l6LCwkFo6hH6IlBn0XI95TqXwmfijmzwI8zSOJToU9MigkNcM4RxNSJc-0Agf_4EAwmnlRUBQTw8UwS_KFLOWv0V5p__rfr6qWSYCRYTK6yO511_u4zGUD9QWsIl3QUTmtwj2EcQ4cE9y0-XFrgcbD72eThu6pSHCB4rSVS9I0H0HHBwdeEbflAFtw8cTjGeTo-nopG0rE7SUhKz1Jy2LWDvnecn0XvtoNwH9M5_Z9lFFbTzC7Tm3ptpAfH9A8zuxsRAVitX7Fz1EP4SbkwbK_P0GuDZ4Nh4LIRHUb-b7Bl6gDuxsiBK6qocW2QSXZw5Z6z3D_amDaLHlOD2e51m8mzVLxU6kvhZDW5VGcWidw9PXP2p6BgKvbltvwvYWIK6Vws8aDT; ASP.NET_SessionId=l5qipqf0tx2rjcubdmlmywrc" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                if (body.Contains("Agendamento - Sele"))
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(body);


                    var viewStateHtml = doc.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']");

                    var viewState = viewStateHtml.GetAttributeValue("value", "");

                    var validationHtml = doc.DocumentNode.SelectSingleNode("//input[@id='__EVENTVALIDATION']");

                    var validationState = validationHtml.GetAttributeValue("value", "");

                    Console.WriteLine("Getting page details...");
                    await selectProcessReCaptcha(viewState, validationState);

                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in select process: " + ex.ToString());
        }
    }

    private static async Task selectProcessReCaptcha(string viewState, string validationState)
    {
        try
        {
            string payload = JsonConvert.SerializeObject(new
            {
                page_url = "https://sapa.sef.pt/Agendamento/SeleccionarProcesso",
                site_key = "6LcJPTUUAAAAAApYt0_OWtPKizfRIlefYYoLhl1G",
                access_token = access_token,
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var clientHandler = new HttpClientHandler
            {
            };
            var client = new HttpClient(clientHandler);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://bcsapi.xyz/api/captcha/recaptcha"),
                Content = content

            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject<dynamic>(body);

                if (json.ContainsKey("status"))
                {
                    string submitted = json.status;

                    if (submitted.Equals("submitted"))
                    {
                        Console.WriteLine("Bypass sent to the server.");
                        int id = json.id;
                        await selectProcessReCaptchaRetrive(id, viewState, validationState);
                    }
                    else
                    {
                        Console.WriteLine("Unable to sort captcha out.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in bypass process: " + ex.ToString());
        }

    }

    private static async Task selectProcessReCaptchaRetrive(int RECAPTCHA_ID, string viewState, string validationState)
    {
        try
        {
            bool retrive = true;

            while (retrive)
            {
                var clientHandler = new HttpClientHandler
                {
                };
                var client = new HttpClient(clientHandler);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://bcsapi.xyz/api/captcha/" + RECAPTCHA_ID + "?access_token=" + access_token),

                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    dynamic json = JsonConvert.DeserializeObject<dynamic>(body);


                    if (json.ContainsKey("gresponse"))
                    {
                        retrive = false;
                        Console.WriteLine("Bypass Accepted!");
                        string gresponse = json.gresponse;
                        await selectProcessPost(viewState, validationState, gresponse);

                    }
                    else
                    {
                        date2 = DateTime.Now;

                        TimeSpan ts = date2.Subtract(date1);

                        if (ts.TotalSeconds > 80)
                        {
                            Console.WriteLine("Too long to retrive response, sending a new request.");
                            retrive = false;
                        }
                        else
                        {
                            Console.WriteLine("Waiting for response..."+ts.TotalSeconds);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in retrive bypass response: " + ex.ToString());
        }

    }

    private static async Task selectProcessPost(string viewState, string eventValidation, string gCaptchaResponse)
    {
        try
        {
            var clientHandler = new HttpClientHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            var client = new HttpClient(clientHandler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://sapa.sef.pt/Agendamento/SeleccionarProcesso"),
                Headers =
    {
        { "host", "sapa.sef.pt" },
        { "connection", "keep-alive" },
        { "cache-control", "max-age=0" },
        { "sec-ch-ua", "\"Not_A Brand\";v=\"99\", \"Google Chrome\";v=\"109\", \"Chromium\";v=\"109\"" },
        { "sec-ch-ua-mobile", "?0" },
        { "sec-ch-ua-platform", "\"Windows\"" },
        { "upgrade-insecure-requests", "1" },
        { "origin", "https://sapa.sef.pt" },
        { "user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36" },
        { "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
        { "sec-fetch-site", "same-origin" },
        { "sec-fetch-mode", "navigate" },
        { "sec-fetch-user", "?1" },
        { "sec-fetch-dest", "document" },
        { "referer", "https://sapa.sef.pt/Agendamento/SeleccionarProcesso" },
        { "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" },
        { "cookie", "__AntiXsrfToken=c5b08f3e174949a69befc634cfa72c34; .ASPXAUTH=BFB10506693421C8015FF4CC5B9CA39D585577B8C03D143B2EFB974E3BB1D163FE4CA8F50DAFE8C72817C5775C005D474308F40C28C70BAD70518E9289B13E80A5CE301F1AC69524BA3717FBFE7A7C1D3CC8D36FEB130ECBCA9C24DA26667A4DB8666A78966EC3E28F05F6E2C395782EF9D3A89FE330B1AD2EB820180FD6E6B2; .AspNet.ApplicationCookie=VaGi1ISyK5ClnmKXxTnnW9efgnjhMsR14tLRW7DvGCAsFFcXsrltc2yVrjvMeyJllX94T4pjzlVsJNM1_A09LTeQG3hYLhHqkppRBI3M3ObG5whKLgEeD27x8l6LCwkFo6hH6IlBn0XI95TqXwmfijmzwI8zSOJToU9MigkNcM4RxNSJc-0Agf_4EAwmnlRUBQTw8UwS_KFLOWv0V5p__rfr6qWSYCRYTK6yO511_u4zGUD9QWsIl3QUTmtwj2EcQ4cE9y0-XFrgcbD72eThu6pSHCB4rSVS9I0H0HHBwdeEbflAFtw8cTjGeTo-nopG0rE7SUhKz1Jy2LWDvnecn0XvtoNwH9M5_Z9lFFbTzC7Tm3ptpAfH9A8zuxsRAVitX7Fz1EP4SbkwbK_P0GuDZ4Nh4LIRHUb-b7Bl6gDuxsiBK6qocW2QSXZw5Z6z3D_amDaLHlOD2e51m8mzVLxU6kvhZDW5VGcWidw9PXP2p6BgKvbltvwvYWIK6Vws8aDT; ASP.NET_SessionId=l5qipqf0tx2rjcubdmlmywrc" },
    },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        { "__EVENTTARGET", "ctl00$MainContent$dg" },
        { "__EVENTARGUMENT", "Select$0" },
        { "__VIEWSTATE", viewState },
        { "__VIEWSTATEGENERATOR", "689C5961" },
        { "__VIEWSTATEENCRYPTED", "" },
        { "__EVENTVALIDATION", eventValidation },
        { "g-recaptcha-response", gCaptchaResponse },
    }),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(body);

                var viewStateHtml = doc.DocumentNode.SelectSingleNode("//span[@id='MainContent_Mensagem']");

                string message = viewStateHtml.InnerText;

                if (message.Contains("o existem vagas disponíveis para Artigo"))
                {
                    Console.WriteLine("No slots available");
                    Console.WriteLine(message);
                }
                else
                {

                    //validate html
                    var viewRHtml = doc.DocumentNode.SelectSingleNode("//label[@id='lblMessage']");

                    if (viewRHtml != null)
                    {

                        string label = viewRHtml.InnerText;

                        if (label.Contains("Valide o controlo"))
                        {
                            Console.WriteLine("Validate again.");
                        }
                        else
                        {
                            Sms.sendSms("Slot available, body saved");
                            using (StreamWriter writetext = new StreamWriter("write" + process + ".txt"))
                            {
                                writetext.WriteLine(body);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Request canceled, sending a new request.");
                    }
                }
                //await loadBookingFirst();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in post process: " + ex.ToString());
        }
    }

    private static async Task loadBookingFirst()
    {
        try
        {
            var clientHandler = new HttpClientHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            var client = new HttpClient(clientHandler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://sapa.sef.pt/Agendamento/Agendar.aspx"),
                Headers =
    {
        { "host", "sapa.sef.pt" },
        { "connection", "keep-alive" },
        { "cache-control", "max-age=0" },
        { "upgrade-insecure-requests", "1" },
        { "user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36" },
        { "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
        { "sec-fetch-site", "same-origin" },
        { "sec-fetch-mode", "navigate" },
        { "sec-fetch-user", "?1" },
        { "sec-fetch-dest", "document" },
        { "sec-ch-ua", "\"Not_A Brand\";v=\"99\", \"Google Chrome\";v=\"109\", \"Chromium\";v=\"109\"" },
        { "sec-ch-ua-mobile", "?0" },
        { "sec-ch-ua-platform", "\"Windows\"" },
        { "referer", "https://sapa.sef.pt/Agendamento/SeleccionarProcesso" },
        { "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" },
        { "cookie", "__AntiXsrfToken=c5b08f3e174949a69befc634cfa72c34; .ASPXAUTH=BFB10506693421C8015FF4CC5B9CA39D585577B8C03D143B2EFB974E3BB1D163FE4CA8F50DAFE8C72817C5775C005D474308F40C28C70BAD70518E9289B13E80A5CE301F1AC69524BA3717FBFE7A7C1D3CC8D36FEB130ECBCA9C24DA26667A4DB8666A78966EC3E28F05F6E2C395782EF9D3A89FE330B1AD2EB820180FD6E6B2; ASP.NET_SessionId=l5qipqf0tx2rjcubdmlmywrc; .AspNet.ApplicationCookie=9KOl2Z1CAU5snghm_SAl9jIdn357lVF2NpLlVZJz0kMqwb43BH8wtI2MYaFRqDavtAdFkI0VOAQ-8WnnH91WsYartooMqzrGdwwZof6_cw5HwCOxeTCOAX3XxdWzq9LBbAlC_yOhL2IAMolchXTSFTNQ-4_6dPVRgQ4nusECG6ezuBnKrCLkcQQQB6zz65gZSKDNl4LLXuKWfyttAeOzhM7XExRqlhLFP-nM6Pa3c81oSG3dLAykfNIu572nidqpswJuRd6sPKJ4fml2oxFPB3OSAsfBBdcHAvI_KR2ATxmsMAHgG-rdrbV73MCzJABVCRa1Mqp4ricntw0XIPP1NKOo0T3xfUpSLIm-FME7XwE7y8_6TWtxEMa4vMVLvId2V0Xy9EtncmDA_0bayaq9ZNEo3ULBHXaDbQU2LYALsdkPSv66Xi2V2rbuVek8DsLaf0uS355oJl3DM-C7Sp2q4nIBi5SYgX5_oAz4tpP3Tu7HgFGgi5r6h_UXOfIOMXz-" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                await loadBooking();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in loading booking: " + ex.ToString());
        }
    }

    private static async Task loadBooking()
    {
        try
        {
            var clientHandler = new HttpClientHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            var client = new HttpClient(clientHandler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://sapa.sef.pt/Agendamento/Agendar"),
                Headers =
    {
        { "host", "sapa.sef.pt" },
        { "connection", "keep-alive" },
        { "cache-control", "max-age=0" },
        { "upgrade-insecure-requests", "1" },
        { "user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36" },
        { "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
        { "sec-fetch-site", "same-origin" },
        { "sec-fetch-mode", "navigate" },
        { "sec-fetch-user", "?1" },
        { "sec-fetch-dest", "document" },
        { "sec-ch-ua", "\"Not_A Brand\";v=\"99\", \"Google Chrome\";v=\"109\", \"Chromium\";v=\"109\"" },
        { "sec-ch-ua-mobile", "?0" },
        { "sec-ch-ua-platform", "\"Windows\"" },
        { "referer", "https://sapa.sef.pt/Agendamento/SeleccionarProcesso" },
        { "accept-language", "en-GB,en-US;q=0.9,en;q=0.8" },
        { "cookie", "__AntiXsrfToken=c5b08f3e174949a69befc634cfa72c34; .ASPXAUTH=BFB10506693421C8015FF4CC5B9CA39D585577B8C03D143B2EFB974E3BB1D163FE4CA8F50DAFE8C72817C5775C005D474308F40C28C70BAD70518E9289B13E80A5CE301F1AC69524BA3717FBFE7A7C1D3CC8D36FEB130ECBCA9C24DA26667A4DB8666A78966EC3E28F05F6E2C395782EF9D3A89FE330B1AD2EB820180FD6E6B2; .AspNet.ApplicationCookie=VaGi1ISyK5ClnmKXxTnnW9efgnjhMsR14tLRW7DvGCAsFFcXsrltc2yVrjvMeyJllX94T4pjzlVsJNM1_A09LTeQG3hYLhHqkppRBI3M3ObG5whKLgEeD27x8l6LCwkFo6hH6IlBn0XI95TqXwmfijmzwI8zSOJToU9MigkNcM4RxNSJc-0Agf_4EAwmnlRUBQTw8UwS_KFLOWv0V5p__rfr6qWSYCRYTK6yO511_u4zGUD9QWsIl3QUTmtwj2EcQ4cE9y0-XFrgcbD72eThu6pSHCB4rSVS9I0H0HHBwdeEbflAFtw8cTjGeTo-nopG0rE7SUhKz1Jy2LWDvnecn0XvtoNwH9M5_Z9lFFbTzC7Tm3ptpAfH9A8zuxsRAVitX7Fz1EP4SbkwbK_P0GuDZ4Nh4LIRHUb-b7Bl6gDuxsiBK6qocW2QSXZw5Z6z3D_amDaLHlOD2e51m8mzVLxU6kvhZDW5VGcWidw9PXP2p6BgKvbltvwvYWIK6Vws8aDT; ASP.NET_SessionId=l5qipqf0tx2rjcubdmlmywrc" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(body);

                var viewStateHtml = doc.DocumentNode.SelectSingleNode("//span[@id='MainContent_Mensagem']");

                if (viewStateHtml != null)
                {

                    string message = viewStateHtml.InnerText;


                    Console.WriteLine(viewStateHtml.InnerText);

                    if (message.Contains("tem o processo seleccionado."))
                    {
                        Console.WriteLine("No slots available");
                    }
                    else
                    {
                        Sms.sendSms("Slot available, body saved");

                        using (StreamWriter writetext = new StreamWriter("write" + process + ".txt"))
                        {
                            writetext.WriteLine(body);
                        }
                    }
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in loading booking: " + ex.ToString());
        }
    }
}
