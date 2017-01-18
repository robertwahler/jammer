require 'nokogiri'

module BasicUnity

  class Compile < Thor
    namespace :compile
    include Thor::Actions
    include BasicUnity::BasicUnityHelper

    # adds :quiet, :skip, :pretent, :force
    add_runtime_options!

    desc "base", "compile ./src/SDD.Base to Assets/Lib/SDD.Base.dll"
    def base
      command = "#{mcs_binary} -recurse:'src/SDD/Base/*.cs' \
                 -lib:/Applications/Unity/Unity.app/Contents/Frameworks/ \
                 -lib:/Applications/Unity/Unity.app/Contents/Frameworks/Managed/ \
                 -r:UnityEngine \
                 -target:library \
                 -out:Assets/Lib/SDD.Base.dll \
                 -doc:Assets/Lib/SDD.Base.xml"
      run(command)
    end

    desc "check", "CLI syntax check"
    def check
      run "#{mcs_binary} @.mcs"
    end

    # used for syntax checking and building dll that can be run through its own unit tests
    desc "mcs", "create .mcs style response file from solution that includes editor and non-editor code and references"
    def mcs
      dll =  File.join(TMP_FOLDER, "SyntaxCheck.dll")
      output =  File.join(ROOT_FOLDER, ".mcs")

      # ensure the tmp folder exists
      FileUtils::mkdir 'tmp' unless File.exists?('tmp')

      lines = []

      # header
      #lines << "-debug"
      lines << "-target:library"
      lines << "-nowarn:0169"
      lines << "-out:'#{dll}'"

      # parse main sln first pass
      lines += parse_csproj(File.join(ROOT_FOLDER, "Assembly-CSharp-firstpass.csproj"))

      # parse editor sln first pass
      lines += parse_csproj(File.join(ROOT_FOLDER, "Assembly-CSharp-Editor-firstpass.csproj"))

      # parse editor sln, but skip the source, we manage that with a 'recurse' since it changes frequently
      lines += parse_csproj(File.join(ROOT_FOLDER, "Assembly-CSharp-Editor.csproj"), options = {:skip_source => true})

      # main sln is done by hand so this script only needs to be run when vendor code changes
      # lines += parse_csproj(File.join(ROOT_FOLDER, "Assembly-CSharp.csproj"))
      #lines << "-recurse:Assets/Editor/*.cs"
      lines << "-recurse:Assets/Test/*.cs"
      lines << "-recurse:Assets/Examples/*.cs" if File.exists?(File.join(ASSETS_FOLDER, "Examples"))
      lines << "-recurse:Assets/Scripts/*.cs"

      # remove dupe assemblies, if any (tvOS, iOS)
      cleaned_lines = []
      dupes = []
      lines.each do |line|
        if line.match(/^\-r:/)
          assembly_name = line.gsub(/.*\//, "")
          if dupes.include?(assembly_name)
            next
          else
            dupes << assembly_name
          end
        end
        cleaned_lines << line
      end

      # remove any dupe lines (mainly defines)
      lines = cleaned_lines.clone
      cleaned_lines = []
      processed = []
      lines.each do |line|
        if processed.include?(line)
          next
        else
          processed << line
        end
        cleaned_lines << line
      end

      # write .mcs file
      File.open(output, 'w') do |file|
        file.write(cleaned_lines.join("\n"))
      end
    end

    private

    # parse XML solution files and return an array of lines to include in an
    # mcs response file
    def parse_csproj(filename, options={})
      lines = []

      if File.exists?(filename)
        File.open(filename, "r") do |file|
          doc = Nokogiri::XML(file)

          unless options[:skip_source]
            # source files
            doc.css('Compile').each do |element|
              name = element.attr("Include")
              name = name.gsub(/\\/, "/") if posix?
              say_status "SOURCE", name, :green
              lines << "'#{name}'"
            end
          end

          # explicitly hinted dll paths
          doc.css('HintPath').each do |element|
            name = element.children.text.chomp
            say_status "DLL", name, :green
            lines << "-r:'#{name}'"
          end

          # compiler defines
          doc.css('DefineConstants').each do |element|
            name = element.children.text.chomp
            name.split(/;/).each do |define|
              say_status "DEFINE", define, :green
              lines << "-define:'#{define}'"
            end
          end
        end
      end

      lines
    end

    def mcs_binary
      unity_root="/Applications/Unity/Unity.app/Contents/Frameworks"
      "#{unity_root}/MonoBleedingEdge/bin/mcs"
    end

    # where to start looking for templates, required by the template methods
    # even though we are using absolute paths
    def self.source_root
      File.dirname(__FILE__)
    end

  end
end

