$(document).ready(function () {
    var diadanhid = '0';
    $("#diadanh").change(function () {
        diadanhid = $(this).children(":selected").attr("value");
    });
    $("#upload").on('click', function (event) {
        event.preventDefault();
        var fd = new FormData();
        var files = $('#file')[0].files[0];
        var checkbox = document.getElementsByName('ldl');
        var isQT = null;
        for (var i = 0; i < checkbox.length; i++) {
            if (checkbox[i].checked === true) {
                isQT = checkbox[i].value;
            }
        }
        fd.append('file', files);
        fd.append('diadanhid', diadanhid);
        fd.append('bc', isQT);

        $.ajax({
            url: '/Reports/ImportExcel',
            type: 'post',
            data: fd,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.status) {
                    alert('Upload thành công');
                } else {
                    alert('Upload thất bại');
                }
            },
        });
    })

});