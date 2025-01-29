#!/usr/bin/env bash

curl --globoff 'https://www.fda.gov/datatables/views/ajax?_drupal_ajax=1&_wrapper_format=drupal_ajax&view_name=recall_solr_index&view_display_id=recall_datatable_block_1&pager_element=0&draw=1&start=0&length=898&columns[0][data]=0&columns[0][searchable]=true&columns[0][orderable]=true&search[value]=&search[regex]=false' | \
    jq -rc '.data | map({date: (.[0] | split("\""))[1], brand: (.[1] | split(">") | .[1] | split("<"))[0], page: (.[1] | split("\""))[1], description: .[2], product_type: .[3], recall_reason: .[4], company: .[5], terminated: (.[6] == "Terminated")})'

