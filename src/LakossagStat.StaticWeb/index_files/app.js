var oevkData;

fetch("./data/lakossag-data.json")
  .then(response => response.json())
  .then(json => {
      oevkData = json;

      document.getElementById('max-increase-abs').textContent = json.MaxIncrease.Difference;
      document.getElementById('max-increase-perc').textContent = json.MaxIncreasePercent.DifferencePercent.toFixed(2);
      document.getElementById('max-decrease-abs').textContent = json.MaxDecrease.Difference;
      document.getElementById('max-decrease-perc').textContent = json.MaxDecreasePercent.DifferencePercent.toFixed(2);
      document.getElementById('stat-refresh-time').textContent = json.Date;

      sortAndDisplayTable(1);
    });

function sortAndDisplayTable(columnIndex){
    if (columnIndex === 0)
        oevkData.OevkItems.sort(dynamicSortByOEVK());
    else if (columnIndex === 1)
        oevkData.OevkItems.sort(dynamicSort('-Difference'));
    else if (columnIndex === 2)
        oevkData.OevkItems.sort(dynamicSort('-DifferencePercent'));
    displayTable();
}

function displayTable(){
    var table = '<div class="row table-header">'
        + '<div class="col-sm"></div>'
        + '<div class="col-4 text-left"><h4 onclick="sortAndDisplayTable(0)" style="cursor: pointer">Választókerület</h4></div>'
        + '<div class="col-3"><h4 onclick="sortAndDisplayTable(1)" style="cursor: pointer">Változás 2021. december óta</h4></div>'
        + '<div class="col-2"><h4 onclick="sortAndDisplayTable(2)" style="cursor: pointer">Százalék</h4></div>'
        + '<div class="col-sm"></div>'
        + '</div>';
      
        oevkData.OevkItems.forEach(item => {
            table += '<div class="row table-text">'
                + '<div class="col-sm"></div>'
                + '<div class="col-4 text-left">' + item.Name + ' ' + item.Index + '</div>'
                + '<div class="col-3 text-center">' + getDifferenceText(item.Difference) + ' fő</div>'
                + '<div class="col-2">' + item.DifferencePercent.toFixed(2) + ' %</div>'
                + '<div class="col-sm"></div>'
                + '</div>';
        });

 
	        document.getElementById("oevk-table").innerHTML = table;
}

function dynamicSort(property) {
    var sortOrder = 1;
    if(property[0] === "-") {
        sortOrder = -1;
        property = property.substr(1);
    }
    return function (a,b) {
        var result = (a[property] < b[property]) ? -1 : (a[property] > b[property]) ? 1 : 0;
        return result * sortOrder;
    }
}
function dynamicSortByOEVK() {
    return function (a,b) {
        var valA = a.Name + a.Index;
        var valB = b.Name + b.Index;
        var result = (valA < valB) ? -1 : (valA > valB) ? 1 : 0;
        return result;
    }
}

function getDifferenceText(diffValue){
    if (diffValue > 0)
        return '+' + diffValue;
    else 
        return diffValue;
}