<template>
    <tr>
        <th v-for="(column, index) in columns" :column="column" :key="index">
            {{ column }}
        </th>
    </tr>
    
</template>

<script>

export default {
  name: 'DashboardTableHeader',
  props: ['columns'],
}
</script>

<style>

</style>