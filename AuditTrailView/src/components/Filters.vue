<template>
  <div id="filters">
    <div class="pure-form a-basic-margin pure-g">
      <fieldset class="options-block pure-u-1-5">
      <legend>Columns </legend>
      <collapse :selected="false">
        <div slot="collapse-header" class="column-header">
          Select visible columns
        </div>
        <div slot="collapse-body">
          <div v-for="(column, index) in columns" :key="index">
            <input type="checkbox" :id="index" :checked="column" @change="toggleColumn(index)" >
            <label :id="index"> {{ index }}</label>
            <br/>
          </div>
        </div>
      </collapse>

      </fieldset>
    <fieldset class="options-block pure-u-4-5" id="filter-options">
        <legend>Filters</legend>

        <collapse :selected="false" class="pure-u-1-4">
        <div slot="collapse-header" class="column-header">
          Field Filters
        </div>
        <div slot="collapse-body">
          <div v-for="(column, index) in fieldFilters" :key="index">
            <label :for="column" class="filter-label">{{ column }}</label><br/>
            <input class="a-basic-margin" type="text" :id="column"  @change="changeFilter(column, $event.target.value)"> 
            <br/>
          </div>
        </div>
      </collapse>

        <input class="a-basic-margin" type="datetime-local" placeholder="Startdate">

        <button class="pure-button">Reset</button>

    </fieldset>
    </div>
  </div>
</template>

<script>
import Collapse from 'vue-collapse'

export default {
  name: 'Filters',
  props: ['columns'],
  components: {
    Collapse
  },
  data () {

    return {
      customFilters:
      [
        "Timestamp"
      ]
    }
  },
  computed: {
     fieldFilters: function() {
          var f = [];
          for (var column in this.columns) {
              if (!this.customFilters.includes(column)) {
                f.push(column);
              }
          }
          return f;
     }
  },
  methods: {
    toggleColumn : function(colName) {
      this.$emit('toggleColumn', colName);
    },
    changeFilter : function(fieldName, value) {
      this.$emit('changeFilter', fieldName, value);
    }
  }
}
</script>

<style>
.a-basic-margin {
    margin: 3px;
}

.column-header {
  max-height:0px;
}

.filter-label {
  font-size: 12px;
}

#filters {
  min-width:1400px;
}





</style>