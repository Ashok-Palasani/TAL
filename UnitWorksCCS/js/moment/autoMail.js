var jq = $.noConflict();
jq(document).ready(function () {
    jq(".MailList").autocomplete({
        source: function (req, resp) {
            jq.ajax({
                url: "/AutomaticEmailReport/GetMailIDs",
                type: "POST",
                dataType: "json",
                data: { Prefix: GetCurrentSearchTerm(req.term), AllVal: req.term },
                success: function (data) {
                    resp(jq.map(data, function (item) {
                        return { label: item.EmailID, value: item.EmailID };
                    }))
                }
            })
        },

        select: function (event, ui) {
            var LastValue = splitCurrentText(this.value);
            LastValue.pop();
            LastValue.push(ui.item.value);
            LastValue.push("");
            this.value = LastValue.join(",");
            return false;
        },
        focus: function () {
            return false;
        }
    });

    function splitCurrentText(LastTerm) {
        return LastTerm.split(/,\s*/);
    }

    function GetCurrentSearchTerm(LastTerm) {
        return splitCurrentText(LastTerm).pop();
    }
});