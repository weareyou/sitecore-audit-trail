<template>
    <tr class="tooltip tooltip-scroll select-hover table-row">
        <td v-for="(field, index) in filteredRecord" :field="index" :key="index">
            {{ field | dynamicFormatFilter }}
        </td>

        <div class="wrapper"> 
            <span class="tooltip-text">
                <div v-if="Object.entries(record.EventData[record.Event]).length == 0">
                    No specific event data.
                </div>
                
                <div v-for="(item, index) in record.EventData[record.Event]" :key="index">
                    <!-- TODO: rather not have this check be hardcoded for Saved events-->
                    <div v-if="index == 'Fields' || index == 'Properties'">
                        <strong>{{ index }}</strong>
                        <div class="indent" v-for="(innerItem, innerIndex) in item" :key="innerIndex">
                            <b>{{ innerIndex }}</b>
                            <div class="indent pure-g" v-for="(savedItem, savedIndex) in innerItem" :key="savedIndex">
                                <div class="pure-u-1-4">{{ savedIndex }}:</div> <div class="pure-u-3-4">{{ savedItem | dynamicFormatFilter }}</div>
                            </div>
                            <br>

                        </div>

                    </div>

                    <div v-else>
                        <div class="pure-u-1-4">{{ index }}:</div> <div class="pure-u-3-4">{{ item}}</div>
                    </div>

                </div>

            </span>
        </div>

    </tr>
</template>

<script>
import moment from 'moment'

export default {
  name: 'dashboard-table-row',
  props: ['record', 'columns'],
  computed: {
        filteredRecord: function() {
          var r = [];
          for (var column in this.columns) {
              var field = this.record[this.columns[column]];

              

              if (this.columns[column] === "Timestamp")
                field = moment(String(this.record[this.columns[column]])).format('MM/DD/YYYY hh:mm');

              r.push(field);
          }
          return r;
      }
  },
  filters: {
      dynamicFormatFilter: function(value) {
          var datetimeExp = /\d{8}T\d{6}Z/g;
          // check if datetime
          if (datetimeExp.test(value)) {
              return moment(String(value)).format('MM/DD/YYYY hh:mm');
          }
        
        return value;
      }
  }
}
</script>

<style>
tr:hover {
   border: 1px inset orange; 
}

.table-row {
    min-height: 50px;
}

.indent
{
    margin-left:15px;
}

.wrapper{
    position:absolute;
    left: 180px;
    width: 620px;
}
.tooltip {
    transform: none;
    margin: 50px;
}

.tooltip:hover > .tooltip-text, .tooltip:hover > .wrapper {
    pointer-events: auto;
    opacity: 0.8;
    margin-top: 30px;
}

.tooltip > .tooltip-text, .tooltip >.wrapper {
    display: block;
    position: absolute;
    z-index: 6000;
    overflow: visible;
    padding: 5px 8px;
    margin-top: 50px;
    line-height: 16px;
    border-radius: 4px;
    text-align: left;
    color: #fff;
    background: #000;
    pointer-events: none;
    opacity: 0.0;
    -o-transition: all 0.3s ease-out;
    -ms-transition: all 0.3s ease-out;
    -moz-transition: all 0.3s ease-out;
    -webkit-transition: all 0.3s ease-out;
    transition: all 0.3s ease-out;
}

/* Arrow */
.tooltip > .tooltip-text:before, .tooltip > .wrapper:before  {
    display: inline;
    top: -5px;
    content: "";
    position: absolute;
    border: solid;
    border-color: rgba(0, 0, 0, 1) transparent;
    border-width: 0 .5em .5em .5em;
    z-index: 6000;
    left: 20px;
}

/* Invisible area so you can hover over tooltip */
.tooltip > .tooltip-text:after, .tooltip > .wrapper:after  {
    top: -20px;
    content: " ";
    display: block;
    height: 20px;
    position: absolute;
    width: 60px;
    left: 20px;
}

.wrapper > .tooltip-text {
    overflow-y: auto;
    max-height: 220px;
    display: block;
}
</style>