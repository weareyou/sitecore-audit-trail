<template>
<table class="pure-table pure-table-horizontal pure-table-striped">
    <thead>
      <DashboardTableHeader :columns="filteredColumns"></DashboardTableHeader>
    </thead>

    <tbody>
      <DashboardTableRow v-for="(record, index) in records" :record="record" :key="index" :columns="filteredColumns"></DashboardTableRow>
    </tbody>
</table>
</template>

<script>
import DashboardTableHeader from './DashboardTableHeader.vue'
import DashboardTableRow from './DashboardTableRow.vue'

export default {
  name: 'DashboardTable',
  components: {
    DashboardTableHeader,
    DashboardTableRow
  },
  props: ['records', 'columns'],
  computed: {
      filteredColumns: function() {
          var c = [];
          for (var column in this.columns) {
              if (this.columns[column] == true) {
                  c.push(column);
              }
          }
          return c;
      }
  }
  
}
</script>

<style>

</style>