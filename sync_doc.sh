

rm -rf doc/manual/ilib-*

TOC="- name: Top\n  href: index.md\n"

for dir in $(ls ./Unity/Assets/ | grep ilib-* | grep -v .meta); do
	cp -r "./Unity/Assets/${dir}/Documentation~" "doc/manual/${dir}"
	TOC="${TOC}- name: ${dir}\n  href: ${dir}/toc.yml\n"
done

echo -e "${TOC}" > doc/manual/toc.yml
echo "# Scripting API" > doc/api/index.md

