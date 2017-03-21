$(document).ready(function() {

          $("#slider2").slider({
              animate: true,
              value: document.getElementById("SpaceTaken").getAttribute("value"),
              min: 0,
              max: 5000,
              step: 1,
              slide: function(event, ui) {
                  update(2,ui.value); //changed
              }
          });

          //Added, set initial value.
          $("#SpaceTaken").val(document.getElementById("SpaceTaken").getAttribute("value"));
          $("#SpaceTaken-label").text(document.getElementById("SpaceTaken").getAttribute("value"));
          
          update();
      });

      //changed. now with parameter
      function update(slider,val) {
        //changed. Now, directly take value from ui.value. if not set (initial, will use current value.)
        var $SpaceTaken = slider == 2?val:$("#SpaceTaken").val();

        /* commented
        $amount = $( "#slider" ).slider( "value" );
        $SpaceTaken = $( "#slider2" ).slider( "value" );
         */

         $( "#SpaceTaken" ).val($SpaceTaken);
         $( "#SpaceTaken-label" ).text($SpaceTaken);

         $('#slider2 a').html('<label><span class="glyphicon glyphicon-chevron-left"></span>' + $SpaceTaken + '<span class="glyphicon glyphicon-chevron-right"></span></label>');
      }
