module BasicUnity

  class Code < Thor
    namespace :code
    include Thor::Actions
    include BasicUnity::BasicUnityHelper

    desc "prune", "Remove empty folders and Unity meta files that point to empty folders"
    method_option "dry-run", :type => :boolean, :desc => "Show what will happen but don't 'rmdir' or 'rm'"
    def prune(folder=nil)
      # bash version, requires GNU find
      #
      # dry-run
      #     cmd = "find ./Assets -type d -empty -exec echo '{}' \; -exec echo '{}.meta' \;"
      #     run(cmd)
      #
      # for real
      #     cmd = "find ./Assets -type d -empty -exec rmdir '{}' \; -exec rm '{}.meta' \;"
      #     run(cmd)
      #
      folder = ASSETS_FOLDER unless folder
      Dir.glob(File.join(folder, "**", "*")).select do |d|
        File.directory?(d)
      end.reverse_each do |d|
        if ((Dir.entries(d) - %w[ . .. ]).empty?)
          puts d
          Dir.rmdir(d) unless options["dry-run"]
          meta = "#{d}.meta"
          if File.exist?(meta)
            puts meta
            File.delete(meta) unless options["dry-run"]
          end
        end
      end
    end

    desc "defines", "report on compiler defines usage frequency, helps detect typos"
    method_option :verbose, :type => :boolean, :desc => "Show vebose information, not just SDD_"
    def defines
      # cs files
      files = Dir.glob(File.join("Assets", '**', '*.cs'))
      # docs
      files << "README.md" if File.exists?("README.md")

      tokens = {}
      files.each do |filename|
        next if filename =~ /\/Vendor\//

        File.readlines(filename).each_with_index do |line, number|
          # upper case words with underbars at least 5 chars long
          line.scan(/\b[A-Z][A-Z0-9_][A-Z0-9_][A-Z0-9_][A-Z0-9_]+\b/).each do |token|
            #if tokens.key?(token)
            token = token.strip
            tokens[token] = (tokens[token] == nil) ? 1 : tokens[token] + 1
          end
        end
      end

      # sort by key
      tokens.sort.to_h.each do |key, value|
        # skip all but SDD defines unless --verbose
        unless options[:verbose]
          next unless key =~ /SDD_/
        end

        if value > 2
          color = :green
        elsif value == 2
          color = :yellow
        else
          color = :red
        end
        say "#{key}  (#{value})", color
      end
    end

    desc "json", "validate json using jsonlin. `pip install demjson`"
    def json

      # txt files
      Dir.glob( File.join("Assets", "Resources", '**', '*.txt')  ).each do |filename|

        next if filename =~ /\/Fonts\//
        next if filename =~ /\/Recordings\//
        next if filename =~ /README.txt/i
        next if filename =~ /LogEmailFooter.txt/i
        next if filename =~ /LogEmailHeader.txt/i

        # Dependency:
        # pip install demjson
        command = "jsonlint --allow comments #{filename}"

        run(command)
      end

    end

    desc "whitespace", "detect whitespace issues"
    method_option :fixit, :type => :boolean, :desc => "Correct whitespace issues with all files"
    def whitespace
      count = 0
      error_messages = []
      @dirty_filenames = []

      # cs files
      Dir.glob( File.join("Assets", "Scripts", '**', '*.cs')  ).each do |filename|
        error_messages << check_for_tab_characters(filename)
        error_messages << check_for_extra_spaces(filename)
        count += 1
      end

      # json files
      Dir.glob( File.join("Assets", "Resources", '**', '*.txt')  ).each do |filename|
        # Skip Vendor assets", i.e. "Resources", "Vendor"
        next if filename =~ /\Vendor\//
        error_messages << check_for_tab_characters(filename)
        error_messages << check_for_extra_spaces(filename)
        count += 1
      end

      if @dirty_filenames.empty?
        say_status "whitespace clean, checked #{count} files", "", :green
      else
        say_status "whitespace errors checked #{count} files", error_messages.compact.join("\n"), :red
        if options[:fixit]
          @dirty_filenames.each do |filename|
            # whitespace at EOL
            command = "sed -i 's/[ \\t]*$//' '#{filename}'"
            run(command)

            # hard tabs anywhere
            command = "sed -i 's/\\t/  /' '#{filename}'"
            run(command)

            # NOTE: Use brew install dos2unix
            # DOS to Unix EOL
            command = "dos2unix '#{filename}'"
            run(command)
          end
        end
      end
    end

    private

    def check_for_tab_characters(filename)
      failing_lines = []
      File.readlines(filename).each_with_index do |line,number|
        failing_lines << number + 1 if line =~ /\t/
      end

      unless failing_lines.empty?
        @dirty_filenames << filename
        "#{filename} has tab characters on lines #{failing_lines.join(', ')}"
      end
    end

    def check_for_extra_spaces(filename)
      # unix EOL
      @eol = "\n"

      failing_lines = []
      File.readlines(filename).each_with_index do |line,number|
        next if line =~ /^\s+#.*\s+#{@eol}$/
        failing_lines << number + 1 if line =~ /\s+#{@eol}$/
      end

      unless failing_lines.empty?
        @dirty_filenames << filename
        "#{filename} has spaces on the EOL on lines #{failing_lines.join(', ')}"
      end
    end

    # where to start looking for templates, required by the template methods
    # even though we are using absolute paths
    def self.source_root
      File.dirname(__FILE__)
    end

  end
end
