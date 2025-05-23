﻿// Ignore Spelling: Captcha

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text;

namespace Service.Contracts;

internal sealed class GenerateCaptchaService : IGenerateCaptchaService
{
    //private readonly ISession _session;
    //public GenerateCaptchaService(IHttpContextAccessor httpContextAccessor)
    //{
    //    _session = httpContextAccessor.HttpContext.Session;
    //}

    //public Task<(string code, byte[] imageBytes, bool isNewCode)> GenerateCaptcha()
    //{
    //    string code = "";
    //    bool isNewCode = false;

    //    if (_session.TryGetValue("CaptchaCode", out byte[] existingCode))
    //    {
    //        code = Encoding.Default.GetString(existingCode);
    //    }
    //    else
    //    {
    //        code = GenerateCode(5);
    //        isNewCode = true;
    //    }

    //    byte[] imageBytes = GenerateCaptchaImage(code);

    //    return Task.FromResult((code, imageBytes, isNewCode));
    //}

    public Task<(string code, byte[] imageBytes)> GenerateCaptcha()
    {
        string code = GenerateCode(5);
        byte[] imageBytes = GenerateCaptchaImage(code);

        return Task.FromResult((code, imageBytes));
    }

    private string GenerateCode(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        Random rand = new Random();

        return new string(
            Enumerable.Range(0, length)
            .Select(_ => chars[rand.Next(chars.Length)])
            .ToArray()
        );
    }

    private byte[] GenerateCaptchaImage(string code)
    {
        int width = 180;
        int height = 60;

        using Image image = new Image<Rgba32>(width, height);
        image.Mutate(ctx =>
        {
            ctx.Fill(Color.White);

            string fontPath = System.IO.Path.Combine(
                AppContext.BaseDirectory, 
                "Fonts", 
                "LiberationSans-Regular.ttf");

            var fontCollection = new FontCollection();
            var fontFamily = fontCollection.Add(fontPath);
            var font = fontFamily.CreateFont(36, FontStyle.Bold);


            if (font is not null)
            {
                ctx.DrawText(code, font, Color.DarkBlue, new PointF(15, 10));
            }

            Random rand = new Random();

            for (int i = 0; i < 5; i++)
            {
                PointF p1 = new PointF(rand.Next(width), rand.Next(height));
                PointF p2 = new PointF(rand.Next(width), rand.Next(height));
                ctx.DrawLine(Color.LightGray, 1, new[] { p1, p2 });
            }

            for (int i = 0; i < 20; i++)
            {
                float x = rand.Next(width);
                float y = rand.Next(height);
                float radius = 3f;

                EllipsePolygon ellipse = new EllipsePolygon(x, y, radius);
                ctx.Draw(Color.LightGray, 1, ellipse);
            }
        });

        using MemoryStream ms = new MemoryStream();
        image.SaveAsPng(ms);

        return ms.ToArray();
    }
}
