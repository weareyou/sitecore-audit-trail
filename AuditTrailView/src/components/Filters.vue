<template>
  <div id="filters">
    <form class="pure-form a-basic-margin pure-g">
      <fieldset class="options-block pure-u-1-5">
      <legend>Columns </legend>
      <multiselect v-model="value" :options="options" :multiple="true" :close-on-select="false" :clear-on-select="false" searchable="false" placeholder="Show/hide columns" label="name" track-by="name"/>
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
import Multiselect from 'vue-multiselect';

export default {
  name: 'Filters',
  props: ['columns'],
  components: {
    Multiselect
  },
  data () {
    return {
      value: [],
      options: [
        { name: 'Vue.js', language: 'JavaScript' },
        { name: 'Adonis', language: 'JavaScript' },
        { name: 'Rails', language: 'Ruby' },
        { name: 'Sinatra', language: 'Ruby' },
        { name: 'Laravel', language: 'PHP' },
        { name: 'Phoenix', language: 'Elixir' }
      ]
    }
  }
}
</script>

<style>
.a-basic-margin {
    margin: 3px;
}

</style>