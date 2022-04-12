window.Toast = {
    CreateToast: function (style, header, message) {
        window.setTimeout(function () {
            var duration = 5000;
            var textColor = '#00ff00';
            var icon = '';

            switch (style) {
                case 'error':
                    textColor = '#db2828';
                    icon = 'red exclamation circle';
                    break;
                case 'warning':
                    textColor = '#ffe21f';
                    icon = 'yellow warning circle';
                    break;
                case 'info':
                    textColor = '#54c8ff';
                    icon = 'blue info cirlce circle';
                    break;
                case 'success':
                    textColor = '#00ff00';
                    icon = 'green check icon';
                    break;
            }

            message = ' \
            <h2 class="ui small inverted header fully-centered"> \
                <i class="large inverted ' + icon + ' icon"></i> \
                <div class="content"> ' + header + ' \
                <div class="sub header">' + message + '</div> \
                </div> \
            </h2>';

            Snackbar.show({
                text: message,
                pos: 'bottom-center',
                duration: duration,
                actionTextColor: textColor
            });
        }, 750);
    }
}
