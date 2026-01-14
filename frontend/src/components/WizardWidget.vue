<script setup>
import { ref, onMounted } from 'vue';
import { LxIcon } from '@wntr/lx-ui';

const props = defineProps({
  graph: {
    type: String,
  },
});

const emits = defineEmits(['node-selected']);

const parsedMermaidList = ref({});
const targetedNodeTitle = ref('');
const targetNodeSummary = ref('');

const previousNode = ref(null);
const previousNodeName = ref(null);
const currentNode = ref(null);
const layoutMarkup = ref(null);

function parseDiagram(input) {
  const lines = input.split('\n');
  const nodes = {};

  lines.forEach((line) => {
    const trimmedLine = line.trim();
    if (!trimmedLine) return;

    const [source, targetPart] = trimmedLine.split('-->');
    const [sourceNode, sourceTitle, sourceSummary] = source.split(/[[\]&]/);
    const [, listItem, target] = targetPart.split('|');
    const [targetNode, targetTitle, targetDescription, targetLayout] = target.split(/[[\]&*]/);

    const trimmedSrcNode = sourceNode.trim();
    const trimmedTrgNode = targetNode.trim();

    if (!nodes[trimmedSrcNode]) {
      nodes[trimmedSrcNode] = {
        node: trimmedSrcNode,
        title: sourceTitle || '',
        listItems: [],
        parentNode: null,
        summary: sourceSummary || null,
      };
    }

    if (!nodes[trimmedTrgNode]) {
      nodes[trimmedTrgNode] = {
        node: trimmedTrgNode,
        title: targetTitle || '',
        listItems: [],
        parentNode: null,
      };
    }

    if (nodes[trimmedTrgNode].parentNode === null) {
      nodes[trimmedTrgNode].parentNode = trimmedSrcNode;
    }

    nodes[trimmedSrcNode].listItems.push({
      item: listItem.trim(),
      itemDescription: targetDescription.trim() || null,
      targetNode: trimmedTrgNode,
      targetTitle: targetTitle.trim(),
      layout: targetLayout.trim() || null,
    });
  });

  return nodes;
}

function goBack(node) {
  const getItem = parsedMermaidList.value[node];

  if (getItem.parentNode) {
    const getPrevItem = parsedMermaidList.value[getItem.parentNode];
    const getPrevTitle = getPrevItem?.listItems?.find((item) => item.targetNode === node);
    previousNodeName.value = getPrevTitle.item;
  }

  previousNode.value = getItem.parentNode;
  currentNode.value = node;
  targetedNodeTitle.value = getItem.title;
  layoutMarkup.value = null;
  targetNodeSummary.value = getItem.summary;
}

function goTo(itemObj) {
  const { item, targetNode, targetTitle, layout } = itemObj;
  const getItem = parsedMermaidList.value[targetNode];

  previousNode.value = getItem.parentNode;
  previousNodeName.value = item;
  currentNode.value = targetNode;
  targetedNodeTitle.value = targetTitle;
  layoutMarkup.value = layout;
  targetNodeSummary.value = null;

  emits('node-selected', targetNode);
}

onMounted(() => {
  const parsedData = parseDiagram(props.graph);
  parsedMermaidList.value = parsedData;
  const objKeys = Object.keys(parsedData);
  const initialListKey = objKeys[0];
  currentNode.value = initialListKey;
  const firstNodeTitle = parsedData[initialListKey].title;
  const firstNodeSummary = parsedData[initialListKey].summary;
  targetedNodeTitle.value = firstNodeTitle;
  targetNodeSummary.value = firstNodeSummary;
});
</script>

<template>
  <div class="instructions">
    <ul v-if="previousNode" class="routes back">
      <li
        tabindex="0"
        @click="goBack(previousNode)"
        @keyup.enter="goBack(previousNode)"
        @keyup.space="goBack(previousNode)"
      >
        <LxIcon value="back" /><label v-html="previousNodeName"></label>
      </li>
    </ul>

    <div v-if="!parsedMermaidList[currentNode]?.parentNode" class="placeholder"></div>
    <p v-if="targetedNodeTitle !== ''">{{ targetedNodeTitle }}</p>

    <ul v-if="!layoutMarkup" class="routes">
      <li
        tabindex="0"
        v-for="(listItem, index) in parsedMermaidList[currentNode]?.listItems"
        :key="index"
        @click="goTo(listItem)"
        @keyup.enter="goTo(listItem)"
        @keyup.space="goTo(listItem)"
      >
        <header>
          <label v-html="listItem.item"></label>
          <p v-if="listItem.itemDescription">
            {{ listItem.itemDescription }}
          </p>
        </header>

        <LxIcon value="next" />
      </li>
    </ul>

    <span v-else v-html="layoutMarkup"></span>
    <span v-if="targetNodeSummary" v-html="targetNodeSummary"></span>
  </div>
</template>
