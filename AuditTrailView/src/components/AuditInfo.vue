<template>
  <div id="audit-info" class="tooltip tooltip-scroll select-hover">
    &#x3f;
    <div class="wrapper"> 
        <span class="tooltip-text">
            This is the Sitecore Audit Trail.
        </span>
    </div>
  </div>
</template>

<script>
export default {
  name: 'AuditInfo'
  
}
</script>

<style>
#audit-info {
    position: absolute;
    top: 5px;
    left: 5px;
    font-size: 18px;
    font-weight: bold;
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
    margin-top: 50px;
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
    max-height: 220px;
    display: block;
}

</style>