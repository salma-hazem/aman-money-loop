using System.Globalization;

namespace MonyLoop.API.Localization;

/// <summary>Localizes presentation text while preserving stable API codes and values.</summary>
internal static class ApiText
{
    private static readonly IReadOnlyDictionary<string, string> Arabic =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["An Unexpected Error Ocurred"] = "حدث خطأ غير متوقع.",
            ["Validation Errors"] = "أخطاء في التحقق من البيانات",
            ["One or more validation errors occurred."] = "حدث خطأ واحد أو أكثر أثناء التحقق من البيانات.",
            ["Error while processing the HTTP request"] = "حدث خطأ أثناء معالجة الطلب",
            ["Please contact support if the problem persists."] = "يرجى التواصل مع الدعم إذا استمرت المشكلة.",
            ["Error while processing the HTTP request - Endpoint Not Found"] = "لم يتم العثور على وجهة الطلب",
            ["The Requested Resource Was Not Found"] = "لم يتم العثور على المورد المطلوب.",
            ["Application not found."] = "لم يتم العثور على طلب العضوية.",
            ["Circle request not found."] = "لم يتم العثور على طلب الجمعية.",
            ["Circle not found."] = "لم يتم العثور على الجمعية.",
            ["Listing not found."] = "لم يتم العثور على الإعلان.",
            ["User was not found."] = "لم يتم العثور على المستخدم.",
            ["No file uploaded."] = "لم يتم رفع ملف.",
            ["Invalid token"] = "رمز التحقق غير صحيح.",
            ["Invalid Egyptian phone number format."] = "صيغة رقم الهاتف المصري غير صحيحة.",
            ["Password confirmation does not match."] = "تأكيد كلمة المرور غير متطابق.",
            ["National ID is required."] = "الرقم القومي مطلوب.",
            ["National ID must be exactly 14 digits."] = "يجب أن يتكون الرقم القومي من 14 رقمًا بالضبط.",
            ["File size must not exceed 5MB."] = "يجب ألا يتجاوز حجم الملف 5 ميجابايت.",
            ["Password must contain an uppercase letter."] = "يجب أن تحتوي كلمة المرور على حرف كبير.",
            ["Password must contain a lowercase letter."] = "يجب أن تحتوي كلمة المرور على حرف صغير.",
            ["Password must contain a digit."] = "يجب أن تحتوي كلمة المرور على رقم.",
            ["Password must contain a special character."] = "يجب أن تحتوي كلمة المرور على رمز خاص.",
            ["Rejection reason is required when rejecting a document."] = "سبب الرفض مطلوب عند رفض المستند.",
            ["Role must be Admin or Organizer."] = "يجب أن يكون الدور مسؤول نظام أو منظمًا.",
            ["OTP code must be exactly 6 digits."] = "يجب أن يتكون رمز التحقق من 6 أرقام بالضبط.",
            ["The new password must be different from the current password."] = "يجب أن تختلف كلمة المرور الجديدة عن الحالية.",
        };

    public static bool IsArabic => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";

    public static string Translate(string source) =>
        IsArabic && Arabic.TryGetValue(source, out var translation) ? translation : source;

    public static string EndpointNotFound(string path) => IsArabic
        ? $"لم يتم العثور على وجهة الطلب {path}."
        : $"Endpoint {path} was not found.";
}
