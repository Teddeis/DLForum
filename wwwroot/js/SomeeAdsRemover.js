$(document).ready(function () {
    SomeeAdsRemover();
});

function SomeeAdsRemover() {
    $("center").each(function () {
        if ($(this).html() == '<a href="http://somee.com">Web hosting by Somee.com</a>') {
            $(this).next().remove();
            $(this).next().next().remove();
            $(this).next().next().next().remove();
            $(this).remove();

            return false;
        }
    });
}