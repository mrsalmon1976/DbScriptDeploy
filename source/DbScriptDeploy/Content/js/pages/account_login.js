$(function () {
    $('#sign_in').validate({
        highlight: function (input) {
            console.log(input);
            $(input).parents('.form-line').addClass('error');
        },
        unhighlight: function (input) {
            $(input).parents('.form-line').removeClass('error');
        },
        errorPlacement: function (error, element) {
            $(element).parents('.input-group').append(error);
        }
    });
});

var loginApp = new Vue({
    el: '#login-app',
    data: {
        isProcessing: false,
        password: '',
        userName: ''
    },
    methods: {
        submitLogin: function () {

            var that = this;
            this.isProcessing = true;
            //this.message = this.message.split('').reverse().join('')
            //debugger;
            //var frm = $('#frm-login');
            //$(that.selectorErrorMessage).addClass('hidden');
            //$(that.selectorLoginButton).prop('disabled', true);
            //$(that.selectorSpinner).removeClass("hide");
            //var formData = {
            //    userName: $(that.selectorUserName).val(),
            //    password: $(that.selectorPassword).val(),
            //};
            var request = $.ajax({
                url: '/api/account/login',
                method: "POST",
                data: {
                    userName: this.userName,
                    password: this.password
                }
            });

            request.always(function (xhr, textStatus, errorThrown) {
                that.isProcessing = false;
            });
            request.done(function (response) {
                window.location.assign('/dashboard');
                //window.location.assign($('#returnUrl').val());
            });
            request.fail(function (xhr, textStatus) {
                alert('TODO: error logging in');
                //    try {
                //        Utils.showError(that.selectorErrorMessage, xhr.responseJSON.message);
                //    }
                //    catch (err) {
                //        Utils.showError(that.selectorErrorMessage, 'A fatal error occurred');
                //    }
            });

        }
    }
});