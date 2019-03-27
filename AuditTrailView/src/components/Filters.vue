<template>
  <div id="filters">
    <form class="pure-form a-basic-margin pure-g">
      <fieldset class="options-block pure-u-1-5">
      <legend>Columns </legend>
      <collapse :selected="false">
        <div slot="collapse-header" class="column-header">
          Select visible columns
        </div>
        <div slot="collapse-body">
          <div v-for="(record, index) in records" :key="index">
          <input type="checkbox" id="record">
          <label for="record">{{record}}</label>
          <br/>
          </div>
        </div>
      </collapse>

      </fieldset>
    <fieldset class="options-block pure-u-4-5" id="filter-options">
        <legend>Filters</legend>
        
          
        <input class="a-basic-margin" type="text" placeholder="Item name or ID">
        <input class="a-basic-margin" type="text" placeholder="Eventname">
        <input class="a-basic-margin" type="text" placeholder="User">
        <input class="a-basic-margin" type="text" placeholder="Startdate (Disables monitor)">

        <button class="pure-button">Reset</button>

    </fieldset>
    </form>
  </div>
</template>

<script>
//import Multiselect from 'vue-multiselect';
import Collapse from 'vue-collapse'

export default {
  name: 'Filters',
  props: ['columns'],
  components: {
    Collapse
  },
  data () {

    return {
    
    }
  },
  computed: {
     
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

</style>