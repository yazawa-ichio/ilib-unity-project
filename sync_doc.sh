

rm -rf doc_source/manual/ilib-*

TOC="- name: Top\n  href: index.md\n"

for dir in $(ls ./Unity/Assets/ | grep ilib-* | grep -v .meta); do
	cp -r "./Unity/Assets/${dir}/Documentation~" "doc_source/manual/${dir}"
	TOC="${TOC}- name: ${dir}\n  href: ${dir}/toc.yml\n"
done

echo -e "${TOC}" > doc_source/manual/toc.yml
echo "# Scripting API" > doc_source/api/index.md

