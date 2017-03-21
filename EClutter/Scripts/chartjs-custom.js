$(document).ready(function () {

    var pieData = [
    {
        value: document.getElementById("AmountCompleted").getAttribute("value") * 1000,
        color: "#339c49",
        label: 'Sorted',
        labelColor: 'white',
        labelFontSize: '16'


    },
    {
        value: (document.getElementById("AmountNotCompleted").getAttribute("value") -1) * 1000 ,
        color: "#F38630",
        label: 'Unsorted',
        labelColor: 'white',
        labelFontSize: '16'

    }

    ];

    var barChartData = {
        
        labels: ["Disposed", "Sold", "Given Away", "Repurposed"],
 
        datasets: [
            {
                fillColor: "rgba(155,155,48,0.5)",
                strokeColor: "rgba(155,155,48,1)",
                data: [document.getElementById("ForDisposal").getAttribute("value"),
                    document.getElementById("ForSale").getAttribute("value"),
                    document.getElementById("ForGivingAway").getAttribute("value"),
                    document.getElementById("ForRepurposing").getAttribute("value")]
            },
            {
                fillColor: "rgba(71, 170, 97,0.5)",
                strokeColor: "rgba(71, 170, 97,1)",
                data: [document.getElementById("Disposed").getAttribute("value"), document.getElementById("Sold").getAttribute("value"), document.getElementById("GivenAway").getAttribute("value"), document.getElementById("Repurposed").getAttribute("value")]
            }
        ]

    };

    new Chart(document.getElementById("bar").getContext("2d")).Bar(barChartData);
    new Chart(document.getElementById("pie").getContext("2d")).Pie(pieData);


});