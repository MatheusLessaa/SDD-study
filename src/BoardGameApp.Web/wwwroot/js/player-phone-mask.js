(function () {
    function formatBrazilianPhone(value) {
        var digits = value.replace(/\D/g, "").slice(0, 11);

        if (digits.length === 0) {
            return "";
        }

        if (digits.length <= 2) {
            return "(" + digits;
        }

        var areaCode = digits.slice(0, 2);
        var phone = digits.slice(2);

        if (phone.length <= 4) {
            return "(" + areaCode + ") " + phone;
        }

        if (digits.length <= 10) {
            return "(" + areaCode + ") " + phone.slice(0, 4) + "-" + phone.slice(4);
        }

        return "(" + areaCode + ") " + phone.slice(0, 1) + " " + phone.slice(1, 5) + "-" + phone.slice(5);
    }

    function bindBrazilianPhoneMask(input) {
        input.addEventListener("input", function () {
            input.value = formatBrazilianPhone(input.value);
        });

        input.value = formatBrazilianPhone(input.value);
    }

    document
        .querySelectorAll("[data-brazilian-phone]")
        .forEach(bindBrazilianPhoneMask);
}());
