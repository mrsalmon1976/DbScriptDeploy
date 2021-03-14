$(document).ready(function () {
    $('#form_script').validate({
        rules: {
            scriptName: {
                required: true,
            },
        },
        onkeyup: function (element) { $(element).valid() },
        highlight: function (input) {
            //console.log(input);
            $(input).parents('.form-line').addClass('error');
        },
        unhighlight: function (input) {
            $(input).parents('.form-line').removeClass('error');
        },
        errorPlacement: function (error, element) {
            $(element).parents('.form-group').append(error);
        }
    });
});

var projectEditorApp = new Vue({
    el: '#editor-page',
    data: {
        projectId: null,
        isSavingScript: false,
        scriptName: '',
        scriptTags: '',

        editorUp: null,
        editorDown: null,
    },
    mounted: function () {

        this.projectId = $('#hid-project-id').val();

        var editorUp = ace.edit("editorUp");
        editorUp.setTheme("ace/theme/sqlserver");
        // set this to /ace/mode/sql if not SqlServer database
        editorUp.session.setMode("ace/mode/sqlserver");
        editorUp.setReadOnly(false); 
        document.getElementById('editorUp').style.fontSize = '14px';
        this.editorUp = editorUp;

        var editorDown = ace.edit("editorDown");
        editorDown.setTheme("ace/theme/sqlserver");
        // set this to /ace/mode/sql if not SqlServer database
        editorDown.session.setMode("ace/mode/sqlserver");
        editorDown.setReadOnly(false); 
        document.getElementById('editorDown').style.fontSize = '14px';
        this.editorDown = editorDown;
    },
    methods: {
        cancel: function () {

        },
        saveScript: function () {
            var that = this;
            var frm = $("#form_script");
            if (frm.valid() && frm.validate().valid()) {
                that.isSavingScript = true;
                var data = {
                    name: that.scriptName,
                    tags: that.scriptTags.split(' '),
                    scriptUp: that.editorUp.getValue(),
                    scriptDown: that.editorDown.getValue()
                };
                var request = $.ajax({
                    url: '/api/project/' + this.projectId + '/script',
                    method: "POST",
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(data) 
                });
                request.done(function (response) {
                    window.location.href = '/project/' + that.projectId;
                });
                request.fail(function (xhr, textStatus, errorThrown) {
                    swal("Error", 'An error occurred saving the script: ' + xhr.statusText, "error");
                });
                request.always(function (xhr, textStatus, errorThrown) {
                    that.isSavingScript = false;
                });
            }
        }
    }
});